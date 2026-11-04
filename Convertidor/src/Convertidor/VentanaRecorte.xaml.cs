using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Convertidor.Conversion;
using Convertidor.Conversion.Paginas;
using ImageMagick;
using Microsoft.Win32;
using Region = Convertidor.Conversion.Paginas.Region;

namespace Convertidor;

/// <summary>
/// Ver un PDF (o una imagen), marcar con el ratón la zona que interesa y guardarla con el
/// formato, la resolución y el tamaño que se quiera.
///
/// Todo se mide en DIP, el tamaño real de la página: el lienzo mide lo mismo que la página y
/// el zoom es una transformación encima. Así la zona marcada no depende del zoom, y los píxeles
/// de salida salen de DIP × ppp / 96.
///
/// La zona se dibuja de nuevo con la resolución elegida, no se recorta de la vista previa: en
/// un PDF con texto o dibujos vectoriales, una zona pequeña a 600 ppp sale nítida.
/// </summary>
public partial class VentanaRecorte : Window
{
    private sealed record OpcionSalida(string Clave, string Nombre, string Extension, bool Alfa, bool ConCalidad)
    {
        public override string ToString() => Nombre; // lo que lee la accesibilidad de Windows
    }

    private static readonly OpcionSalida[] Salidas =
    {
        new("png",  "PNG",            ".png",  Alfa: true,  ConCalidad: false),
        new("jpg",  "JPG",            ".jpg",  Alfa: false, ConCalidad: true),
        new("webp", "WEBP",           ".webp", Alfa: true,  ConCalidad: true),
        new("tiff", "TIFF",           ".tiff", Alfa: true,  ConCalidad: false),
        new("bmp",  "BMP",            ".bmp",  Alfa: false, ConCalidad: false),
        new("gif",  "GIF",            ".gif",  Alfa: true,  ConCalidad: false),
        new("ico",  "Icono (.ico)",   ".ico",  Alfa: true,  ConCalidad: false),
    };

    // Lo último elegido se recuerda mientras el programa está abierto
    private static string _ultimoFormato = "png";
    private static double _ultimaCalidad = 92;

    private const int MaxLado = 20000;
    private const long MaxPixeles = 200_000_000;

    private readonly string _ruta;
    private readonly ObservableCollection<Miniatura> _miniaturas = new();
    private IFuentePaginas? _fuente;
    private int _pagina = -1;
    private double _pw, _ph;           // página en DIP
    private Region? _sel;              // null = página entera
    private double _ppp = 200;
    private int _pxAncho, _pxAlto;
    private double _zoom = 1;
    private bool _zoomAjustado = true; // si se reajusta solo al cambiar el tamaño de la ventana
    private bool _vistaTransparente;
    private bool _actualizando, _ocupado;
    private string? _ultimoGuardado;

    // Arrastre
    private enum Modo { Nada, Nuevo, Mover, Asa }
    private Modo _modo;
    private Point _inicio;
    private Rect _rectInicio;
    private string _asa = "";
    private readonly Dictionary<string, Rectangle> _asas = new();

    public VentanaRecorte(string ruta)
    {
        InitializeComponent();
        _ruta = ruta;
        Title = $"Recortar · {System.IO.Path.GetFileName(ruta)}";
        Miniaturas.ItemsSource = _miniaturas;

        Formato.ItemsSource = Salidas;
        Formato.SelectedItem = Salidas.FirstOrDefault(s => s.Clave == _ultimoFormato) ?? Salidas[0];
        Calidad.Value = _ultimaCalidad;

        foreach (var (asa, cursor) in new[]
                 {
                     ("nw", Cursors.SizeNWSE), ("n", Cursors.SizeNS), ("ne", Cursors.SizeNESW), ("e", Cursors.SizeWE),
                     ("se", Cursors.SizeNWSE), ("s", Cursors.SizeNS), ("sw", Cursors.SizeNESW), ("w", Cursors.SizeWE),
                 })
        {
            var r = new Rectangle { Tag = asa, Fill = Brushes.White, Stroke = (Brush)FindResource("Tinta"), Cursor = cursor };
            _asas[asa] = r;
            Capa.Children.Add(r);
        }
        Dibujar();
    }

    // =====================================================================================
    // Abrir
    // =====================================================================================

    private async void Ventana_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _fuente = await Fuentes.Abrir(_ruta);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"No se ha podido abrir {System.IO.Path.GetFileName(_ruta)}: {ex.Message}",
                "Convertidor", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
            return;
        }

        _ppp = _fuente.PppPropuesta;
        Transparente.Visibility = _fuente.AdmiteFondoTransparente ? Visibility.Visible : Visibility.Collapsed;

        if (_fuente.Paginas == 1) ColMiniaturas.Width = new GridLength(0);
        for (var i = 0; i < _fuente.Paginas; i++)
        {
            var (w, h) = _fuente.Tamano(i);
            _miniaturas.Add(new Miniatura(i, Math.Min(170, 120 * h / w)));
        }

        Miniaturas.SelectedIndex = 0; // carga la primera página
        await CargarMiniaturas();
    }

    private async Task CargarMiniaturas()
    {
        if (_fuente == null || _fuente.Paginas == 1) return;
        foreach (var m in _miniaturas)
        {
            try
            {
                var (w, h) = _fuente.Tamano(m.Pagina);
                var px = 240; // el doble de lo que se ve, para que no salga borrosa
                m.Imagen = ABitmap(await _fuente.DibujarPng(m.Pagina, Region.Entera, px, Math.Max(1, (int)(px * h / w)), false));
            }
            catch { /* una miniatura que falle no impide trabajar */ }
            if (!IsLoaded) return;
        }
    }

    private async void Miniaturas_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Miniaturas.SelectedItem is Miniatura m && m.Pagina != _pagina) await CargarPagina(m.Pagina);
    }

    private async Task CargarPagina(int pagina)
    {
        if (_fuente == null) return;
        _pagina = pagina;
        (_pw, _ph) = _fuente.Tamano(pagina);
        _sel = null;

        foreach (FrameworkElement el in new FrameworkElement[] { Vista, Damero, Capa })
        {
            el.Width = _pw;
            el.Height = _ph;
        }

        await RefrescarVista();
        Cargando.Visibility = Visibility.Collapsed;
        AjustarVista();
        Recalcular();
    }

    /// <summary>
    /// La vista previa se dibuja a unos 3000 px por el lado largo (más si la página es pequeña),
    /// para que al acercar se siga viendo bien.
    /// </summary>
    private async Task RefrescarVista()
    {
        if (_fuente == null) return;
        _vistaTransparente = FondoTransparente;
        var escala = Math.Min(6, 3000 / Math.Max(_pw, _ph));
        try
        {
            var png = await _fuente.DibujarPng(_pagina, Region.Entera,
                Math.Max(1, (int)(_pw * escala)), Math.Max(1, (int)(_ph * escala)), _vistaTransparente);
            Vista.Source = ABitmap(png);
        }
        catch (Exception ex)
        {
            MostrarAviso("No se ha podido dibujar la página: " + ex.Message);
        }
    }

    // =====================================================================================
    // Zoom
    // =====================================================================================

    private void PonerZoom(double z)
    {
        _zoom = Math.Clamp(z, 0.05, 16);
        Escala.ScaleX = Escala.ScaleY = _zoom;
        TextoZoom.Text = $"{_zoom * 100:0} %";
        Dibujar();
    }

    private void AjustarVista()
    {
        if (_pw <= 0) return;
        _zoomAjustado = true;
        var ancho = (Visor.ViewportWidth > 0 ? Visor.ViewportWidth : Visor.ActualWidth) - 48;
        var alto = (Visor.ViewportHeight > 0 ? Visor.ViewportHeight : Visor.ActualHeight) - 48;
        if (ancho <= 0 || alto <= 0) return;
        PonerZoom(Math.Min(6, Math.Min(ancho / _pw, alto / _ph)));
    }

    private void Visor_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_zoomAjustado) AjustarVista();
    }

    private void Visor_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) == 0) return;
        _zoomAjustado = false;
        PonerZoom(_zoom * (e.Delta > 0 ? 1.15 : 1 / 1.15));
        e.Handled = true;
    }

    private void Acercar_Click(object sender, RoutedEventArgs e) { _zoomAjustado = false; PonerZoom(_zoom * 1.25); }
    private void Alejar_Click(object sender, RoutedEventArgs e) { _zoomAjustado = false; PonerZoom(_zoom / 1.25); }
    private void AjustarVista_Click(object sender, RoutedEventArgs e) => AjustarVista();
    private void Zoom100_Click(object sender, RoutedEventArgs e) { _zoomAjustado = false; PonerZoom(1); }

    // =====================================================================================
    // Marcar la zona con el ratón
    // =====================================================================================

    private Rect SelDip => _sel == null
        ? new Rect(0, 0, _pw, _ph)
        : new Rect(_sel.X * _pw, _sel.Y * _ph, _sel.Ancho * _pw, _sel.Alto * _ph);

    private Region ANormal(Rect r) => new(r.X / _pw, r.Y / _ph, r.Width / _pw, r.Height / _ph);

    private Point Dentro(Point p) => new(Math.Clamp(p.X, 0, _pw), Math.Clamp(p.Y, 0, _ph));

    private void Capa_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (_fuente == null || _ocupado) return;
        var p = Dentro(e.GetPosition(Capa));

        if (e.OriginalSource is Rectangle { Tag: string asa } && _sel != null)
        {
            _modo = Modo.Asa;
            _asa = asa;
            _rectInicio = SelDip;
        }
        else if (_sel != null && SelDip.Contains(p))
        {
            _modo = Modo.Mover;
            _inicio = p;
            _rectInicio = SelDip;
        }
        else
        {
            _modo = Modo.Nuevo;
            _inicio = p;
        }
        Capa.CaptureMouse();
        e.Handled = true;
    }

    private void Capa_MouseMove(object sender, MouseEventArgs e)
    {
        var p = Dentro(e.GetPosition(Capa));

        if (_modo == Modo.Nada)
        {
            Capa.Cursor = _sel != null && SelDip.Contains(p) ? Cursors.SizeAll : Cursors.Cross;
            return;
        }

        Rect r;
        switch (_modo)
        {
            case Modo.Nuevo:
                r = new Rect(_inicio, p);
                if (r.Width <= 0 || r.Height <= 0) return;
                break;

            case Modo.Mover:
                var x = Math.Clamp(_rectInicio.X + p.X - _inicio.X, 0, _pw - _rectInicio.Width);
                var y = Math.Clamp(_rectInicio.Y + p.Y - _inicio.Y, 0, _ph - _rectInicio.Height);
                r = new Rect(x, y, _rectInicio.Width, _rectInicio.Height);
                break;

            default: // asa: se mueven solo los bordes que indica ("nw" = arriba e izquierda)
                double izq = _rectInicio.Left, der = _rectInicio.Right, arr = _rectInicio.Top, aba = _rectInicio.Bottom;
                var minimo = 2 / _zoom;
                if (_asa.Contains('w')) izq = Math.Min(p.X, der - minimo);
                if (_asa.Contains('e')) der = Math.Max(p.X, izq + minimo);
                if (_asa.Contains('n')) arr = Math.Min(p.Y, aba - minimo);
                if (_asa.Contains('s')) aba = Math.Max(p.Y, arr + minimo);
                r = new Rect(new Point(izq, arr), new Point(der, aba));
                break;
        }

        _sel = ANormal(r);
        Dibujar();
        Recalcular();
    }

    private void Capa_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_modo == Modo.Nuevo && _sel != null)
        {
            // Un clic sin arrastrar quita la zona
            var r = SelDip;
            if (r.Width * _zoom < 4 || r.Height * _zoom < 4) _sel = null;
        }
        else if (_modo == Modo.Nuevo)
        {
            _sel = null;
        }
        Capa.ReleaseMouseCapture();
        Dibujar();
        Recalcular();
    }

    private void Capa_LostMouseCapture(object sender, MouseEventArgs e) => _modo = Modo.Nada;

    /// <summary>La sombra fuera de la zona, el marco y las ocho asas.</summary>
    private void Dibujar()
    {
        var hay = _sel != null && _pw > 0;
        var vis = hay ? Visibility.Visible : Visibility.Collapsed;
        Mascara.Visibility = MarcoClaro.Visibility = MarcoOscuro.Visibility = vis;
        foreach (var a in _asas.Values) a.Visibility = vis;
        if (!hay) return;

        var r = SelDip;
        Mascara.Data = new CombinedGeometry(GeometryCombineMode.Exclude,
            new RectangleGeometry(new Rect(0, 0, _pw, _ph)), new RectangleGeometry(r));

        // Grosores divididos por el zoom: en pantalla se ven siempre iguales
        var grosor = 1.5 / _zoom;
        foreach (var m in new[] { MarcoClaro, MarcoOscuro })
        {
            m.StrokeThickness = grosor;
            Canvas.SetLeft(m, r.X);
            Canvas.SetTop(m, r.Y);
            m.Width = r.Width;
            m.Height = r.Height;
        }

        var lado = 9 / _zoom;
        foreach (var (clave, asa) in _asas)
        {
            var x = clave.Contains('w') ? r.Left : clave.Contains('e') ? r.Right : r.Left + r.Width / 2;
            var y = clave.Contains('n') ? r.Top : clave.Contains('s') ? r.Bottom : r.Top + r.Height / 2;
            asa.Width = asa.Height = lado;
            asa.StrokeThickness = 1 / _zoom;
            Canvas.SetLeft(asa, x - lado / 2);
            Canvas.SetTop(asa, y - lado / 2);
        }
    }

    private void PaginaEntera_Click(object sender, RoutedEventArgs e) => QuitarZona();

    private void QuitarZona()
    {
        _sel = null;
        Dibujar();
        Recalcular();
    }

    /// <summary>
    /// Quita los márgenes en blanco de la zona (o de la página): dibuja la zona, Magick busca
    /// dónde empieza y acaba el contenido, y la zona se ajusta a eso.
    /// </summary>
    private async void AjustarContenido_Click(object sender, RoutedEventArgs e)
    {
        if (_fuente == null || _ocupado) return;
        var r = SelDip;
        var escala = Math.Min(4, 1500 / Math.Max(r.Width, r.Height));
        var w = Math.Max(1, (int)(r.Width * escala));
        var h = Math.Max(1, (int)(r.Height * escala));

        Ocupado(true, "Buscando el contenido…");
        try
        {
            var png = await _fuente.DibujarPng(_pagina, _sel ?? Region.Entera, w, h, false);
            var caja = await Task.Run(() =>
            {
                using var img = new MagickImage(png);
                img.BackgroundColor = MagickColors.White;
                img.Alpha(AlphaAction.Remove);
                img.ColorFuzz = new Percentage(8);
                img.Trim();
                return (img.Page.X, img.Page.Y, (int)img.Width, (int)img.Height);
            });

            if (caja.Item3 <= 1 && caja.Item4 <= 1)
            {
                Ocupado(false, "La zona está en blanco: no hay nada que ajustar.");
                return;
            }

            var f = r.Width / w; // de píxeles de esta imagen a DIP
            _sel = ANormal(new Rect(r.X + caja.X * f, r.Y + caja.Y * f, caja.Item3 * f, caja.Item4 * f));
            Dibujar();
            Recalcular();
            Ocupado(false, "");
        }
        catch (Exception ex)
        {
            Ocupado(false, "");
            MostrarAviso("No se ha podido ajustar: " + ex.Message);
        }
    }

    // =====================================================================================
    // Resolución y tamaño
    // =====================================================================================

    /// <summary>Con la resolución actual, cuántos píxeles salen de la zona.</summary>
    private void Recalcular()
    {
        var r = SelDip;
        if (r.Width <= 0) return;
        _pxAncho = Fuentes.Px(r.Width, _ppp);
        _pxAlto = Fuentes.Px(r.Height, _ppp);
        MostrarValores();
    }

    private void MostrarValores()
    {
        _actualizando = true;
        PxAncho.Text = _pxAncho.ToString();
        PxAlto.Text = _pxAlto.ToString();
        Resolucion.Text = Math.Round(_ppp, _ppp < 10 ? 1 : 0).ToString(CultureInfo.CurrentCulture);
        _actualizando = false;

        string Cm(double dip) => (dip / 96 * 2.54).ToString("0.0", CultureInfo.CurrentCulture);
        var r = SelDip;
        TextoZona.Text = _sel == null
            ? $"Página entera ({Cm(_pw)} × {Cm(_ph)} cm)"
            : $"{Cm(r.Width)} × {Cm(r.Height)} cm de una página de {Cm(_pw)} × {Cm(_ph)} cm";
        if (_fuente is { Paginas: > 1 }) TextoZona.Text = $"Página {_pagina + 1} de {_fuente.Paginas} · " + TextoZona.Text;

        var mp = (double)_pxAncho * _pxAlto / 1_000_000;
        TextoMedida.Text = $"{(mp < 0.1 ? mp.ToString("0.00") : mp.ToString("0.0"))} megapíxeles · "
                           + $"impreso a {Math.Round(_ppp)} ppp mide {Cm(_pxAncho * 96 / _ppp)} × {Cm(_pxAlto * 96 / _ppp)} cm";

        var demasiado = _pxAncho > MaxLado || _pxAlto > MaxLado || (long)_pxAncho * _pxAlto > MaxPixeles;
        MostrarAviso(demasiado ? $"Demasiado grande: como mucho {MaxLado:N0} px por lado. Baja la resolución." : null);
        BotonGuardar.IsEnabled = BotonCopiar.IsEnabled = !demasiado && !_ocupado && _fuente != null;
    }

    private static bool LeerNumero(string texto, out double valor) =>
        double.TryParse(texto.Replace('.', ','), NumberStyles.Float, CultureInfo.GetCultureInfo("es-ES"), out valor) && valor > 0;

    private void Resolucion_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_actualizando || Resolucion.SelectedItem is not ComboBoxItem { Content: string t }) return;
        if (LeerNumero(t, out var v)) { _ppp = v; Recalcular(); }
    }

    private void Resolucion_Confirmar(object sender, RoutedEventArgs e)
    {
        if (_actualizando) return;
        if (LeerNumero(Resolucion.Text, out var v) && v <= 4800) { _ppp = v; Recalcular(); }
        else MostrarValores(); // valor no válido: se vuelve al anterior
    }

    private void Resolucion_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { Resolucion_Confirmar(sender, e); e.Handled = true; }
    }

    private void PxAncho_Confirmar(object sender, RoutedEventArgs e)
    {
        if (_actualizando) return;
        if (!int.TryParse(PxAncho.Text, out var w) || w < 1) { MostrarValores(); return; }
        var r = SelDip;
        _pxAncho = w;
        _ppp = w * 96 / r.Width;
        if (Proporcion.IsChecked == true) _pxAlto = Math.Max(1, (int)Math.Round(w * r.Height / r.Width));
        MostrarValores();
    }

    private void PxAlto_Confirmar(object sender, RoutedEventArgs e)
    {
        if (_actualizando) return;
        if (!int.TryParse(PxAlto.Text, out var h) || h < 1) { MostrarValores(); return; }
        var r = SelDip;
        _pxAlto = h;
        if (Proporcion.IsChecked == true)
        {
            _ppp = h * 96 / r.Height;
            _pxAncho = Math.Max(1, (int)Math.Round(h * r.Width / r.Height));
        }
        MostrarValores();
    }

    private void Px_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (sender == PxAncho) PxAncho_Confirmar(sender, e); else PxAlto_Confirmar(sender, e);
        e.Handled = true;
    }

    private void Proporcion_Changed(object sender, RoutedEventArgs e)
    {
        // Al volver a bloquearla, el alto se recoloca a partir del ancho
        if (IsLoaded && Proporcion.IsChecked == true) Recalcular();
    }

    // =====================================================================================
    // Formato
    // =====================================================================================

    private OpcionSalida Salida => (OpcionSalida)Formato.SelectedItem;

    private bool FondoTransparente =>
        _fuente is { AdmiteFondoTransparente: true } && Salida.Alfa && Transparente.IsChecked == true;

    private async void Formato_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _ultimoFormato = Salida.Clave;
        PanelCalidad.Visibility = Salida.ConCalidad ? Visibility.Visible : Visibility.Collapsed;
        Transparente.IsEnabled = Salida.Alfa;
        if (_fuente != null && FondoTransparente != _vistaTransparente) await RefrescarVista();
    }

    private void Calidad_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _ultimaCalidad = Calidad.Value;
        if (TextoCalidad != null) TextoCalidad.Text = $"{Calidad.Value:0}";
    }

    private async void Transparente_Changed(object sender, RoutedEventArgs e)
    {
        if (_fuente != null && FondoTransparente != _vistaTransparente) await RefrescarVista();
    }

    // =====================================================================================
    // Guardar y copiar
    // =====================================================================================

    private string NombrePropuesto()
    {
        var nombre = RutaSalida.NombreBase(_ruta);
        if (_fuente is { Paginas: > 1 }) nombre += $" - página {_pagina + 1}";
        if (_sel != null) nombre += " (recorte)";
        return nombre + Salida.Extension;
    }

    private async void Guardar_Click(object sender, RoutedEventArgs e) => await Guardar();

    private async Task Guardar()
    {
        if (_fuente == null || _ocupado || !BotonGuardar.IsEnabled) return;
        var salida = Salida;

        var dlg = new SaveFileDialog
        {
            Title = "Guardar la imagen",
            FileName = NombrePropuesto(),
            DefaultExt = salida.Extension,
            Filter = $"{salida.Nombre}|*{salida.Extension}",
            InitialDirectory = _ultimoGuardado != null
                ? System.IO.Path.GetDirectoryName(_ultimoGuardado)
                : System.IO.Path.GetDirectoryName(_ruta),
            OverwritePrompt = true,
        };
        if (dlg.ShowDialog(this) != true) return;

        var destino = dlg.FileName;
        Ocupado(true, "Guardando…");
        try
        {
            var png = await _fuente.DibujarPng(_pagina, _sel ?? Region.Entera, _pxAncho, _pxAlto, FondoTransparente);
            var ppp = _ppp;
            var calidad = (uint)Calidad.Value;
            await Task.Run(() =>
            {
                using var img = new MagickImage(png) { Density = new Density(ppp, ppp, DensityUnit.PixelsPerInch) };
                ConversorImagen.Guardar(img, ConversorImagen.Formato(salida.Clave), destino, calidad);
            });

            _ultimoGuardado = destino;
            BotonAbrirCarpeta.Visibility = Visibility.Visible;
            Ocupado(false, $"Guardado: {System.IO.Path.GetFileName(destino)} ({_pxAncho} × {_pxAlto} px)");
        }
        catch (Exception ex)
        {
            Ocupado(false, "");
            MostrarAviso("No se ha podido guardar: " + ex.Message);
        }
    }

    private async void Copiar_Click(object sender, RoutedEventArgs e) => await Copiar();

    private async Task Copiar()
    {
        if (_fuente == null || _ocupado || !BotonCopiar.IsEnabled) return;
        Ocupado(true, "Copiando…");
        try
        {
            // El portapapeles de Windows no guarda bien la transparencia: se copia con fondo blanco
            var png = await _fuente.DibujarPng(_pagina, _sel ?? Region.Entera, _pxAncho, _pxAlto, false);
            Clipboard.SetImage(ABitmap(png));
            Ocupado(false, $"Copiada al portapapeles ({_pxAncho} × {_pxAlto} px).");
        }
        catch (Exception ex)
        {
            Ocupado(false, "");
            MostrarAviso("No se ha podido copiar: " + ex.Message);
        }
    }

    private void AbrirCarpeta_Click(object sender, RoutedEventArgs e)
    {
        if (_ultimoGuardado != null) Process.Start("explorer.exe", $"/select,\"{_ultimoGuardado}\"");
    }

    // =====================================================================================
    // Teclado
    // =====================================================================================

    private async void Ventana_KeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.FocusedElement is TextBox) return; // escribiendo un número: que las teclas sean suyas
        var ctrl = (Keyboard.Modifiers & ModifierKeys.Control) != 0;

        switch (e.Key)
        {
            case Key.Escape when _modo == Modo.Nada: QuitarZona(); break;
            case Key.A when ctrl: QuitarZona(); break;
            case Key.C when ctrl: await Copiar(); break;
            case Key.S when ctrl: await Guardar(); break;
            case Key.D0 or Key.NumPad0 when ctrl: AjustarVista(); break;
            case Key.PageDown when Miniaturas.SelectedIndex < _miniaturas.Count - 1: Miniaturas.SelectedIndex++; break;
            case Key.PageUp when Miniaturas.SelectedIndex > 0: Miniaturas.SelectedIndex--; break;
            default: return;
        }
        e.Handled = true;
    }

    // =====================================================================================
    // Varios
    // =====================================================================================

    private void Ocupado(bool ocupado, string estado)
    {
        _ocupado = ocupado;
        Estado.Text = estado;
        if (ocupado) MostrarAviso(null);
        MostrarValores(); // recalcula qué botones se pueden pulsar
    }

    private void MostrarAviso(string? texto)
    {
        Aviso.Text = texto ?? "";
        Aviso.Visibility = string.IsNullOrEmpty(texto) ? Visibility.Collapsed : Visibility.Visible;
    }

    private static BitmapSource ABitmap(byte[] png)
    {
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.StreamSource = new MemoryStream(png);
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
    }

    protected override void OnClosed(EventArgs e)
    {
        _fuente?.Dispose();
        base.OnClosed(e);
    }
}
