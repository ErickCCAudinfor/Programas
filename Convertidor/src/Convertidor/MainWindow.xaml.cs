using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Convertidor.Conversion;
using Convertidor.Conversion.Paginas;
using Microsoft.Win32;

namespace Convertidor;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<ElementoArchivo> _archivos = new();
    private readonly ObservableCollection<OpcionFormato> _formatos = new();
    private string? _clave;
    private bool _trabajando;

    public MainWindow()
    {
        InitializeComponent();
        Lista.ItemsSource = _archivos;
        Formatos.ItemsSource = _formatos;
        Title = $"Convertidor {Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)}";
        Actualizar();
    }

    // =====================================================================================
    // Añadir archivos
    // =====================================================================================

    public void Anadir(IEnumerable<string> rutas)
    {
        var ya = new HashSet<string>(_archivos.Select(a => a.Ruta), StringComparer.OrdinalIgnoreCase);

        foreach (var ruta in Expandir(rutas))
        {
            if (!ya.Add(ruta)) continue;
            _archivos.Add(new ElementoArchivo(ruta));
        }
        RefrescarFormatos();
        Actualizar();
    }

    /// <summary>Una carpeta arrastrada cuenta como todos sus archivos, subcarpetas incluidas.</summary>
    private static IEnumerable<string> Expandir(IEnumerable<string> rutas)
    {
        foreach (var r in rutas)
        {
            if (File.Exists(r)) yield return Path.GetFullPath(r);
            else if (Directory.Exists(r))
                foreach (var f in Directory.EnumerateFiles(r, "*", SearchOption.AllDirectories).OrderBy(x => x))
                    yield return f;
        }
    }

    private void Anadir_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog { Multiselect = true, Title = "Archivos a convertir" };
        if (dlg.ShowDialog(this) == true) Anadir(dlg.FileNames);
    }

    private void Ventana_DragOver(object sender, DragEventArgs e)
    {
        var vale = !_trabajando && e.Data.GetDataPresent(DataFormats.FileDrop);
        e.Effects = vale ? DragDropEffects.Copy : DragDropEffects.None;
        Zona.BorderBrush = (System.Windows.Media.Brush)FindResource(vale ? "Tinta" : "Borde");
        e.Handled = true;
    }

    private void Ventana_DragLeave(object sender, DragEventArgs e) =>
        Zona.BorderBrush = (System.Windows.Media.Brush)FindResource("Borde");

    private void Ventana_Drop(object sender, DragEventArgs e)
    {
        Zona.BorderBrush = (System.Windows.Media.Brush)FindResource("Borde");
        if (_trabajando) return;
        if (e.Data.GetData(DataFormats.FileDrop) is string[] rutas) Anadir(rutas);
    }

    // =====================================================================================
    // Quitar
    // =====================================================================================

    private void Quitar_Click(object sender, RoutedEventArgs e) => QuitarSeleccion();

    private void Vaciar_Click(object sender, RoutedEventArgs e)
    {
        _archivos.Clear();
        RefrescarFormatos();
        Actualizar();
    }

    private void Lista_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete) QuitarSeleccion();
    }

    private void QuitarSeleccion()
    {
        if (_trabajando) return;
        foreach (var a in Lista.SelectedItems.Cast<ElementoArchivo>().ToList()) _archivos.Remove(a);
        RefrescarFormatos();
        Actualizar();
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e) => Actualizar();

    // =====================================================================================
    // Recortar
    // =====================================================================================

    private ElementoArchivo? Recortable =>
        Lista.SelectedItems.Count == 1 && Lista.SelectedItem is ElementoArchivo a && Fuentes.EsRecortable(a.Ruta) ? a : null;

    private void Recortar_Click(object sender, RoutedEventArgs e)
    {
        if (Recortable is not { } a) return;
        // No modal: se pueden tener varias abiertas y seguir usando la principal
        new VentanaRecorte(a.Ruta) { Owner = this }.Show();
    }

    private void Menu_Opened(object sender, RoutedEventArgs e)
    {
        MenuRecortar.IsEnabled = Recortable != null;
        MenuCarpeta.IsEnabled = Lista.SelectedItems.Count == 1;
    }

    private void MenuCarpeta_Click(object sender, RoutedEventArgs e) => Lista_MouseDoubleClick(sender, null!);

    private void Lista_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (Lista.SelectedItem is not ElementoArchivo a) return;
        var ruta = a.Resultado != null && (File.Exists(a.Resultado) || Directory.Exists(a.Resultado)) ? a.Resultado : a.Ruta;
        Process.Start("explorer.exe", $"/select,\"{ruta}\"");
    }

    // =====================================================================================
    // Formatos y destino
    // =====================================================================================

    /// <summary>
    /// Se ofrecen todos los formatos que vale para al menos un archivo. Los que no valen para
    /// todos dicen "PDF · 2 de 3"; al convertir, a los que no lo admiten se les avisa y ya.
    /// Van primero los que valen para más archivos.
    /// </summary>
    private void RefrescarFormatos()
    {
        var admitidos = _archivos.Where(a => a.Destinos.Count > 0).ToList();
        var opciones = admitidos
            .SelectMany(a => a.Destinos)
            .Select((d, orden) => (d, orden))
            .GroupBy(x => x.d.Clave)
            .Select(g => new
            {
                Clave = g.Key,
                g.First().d.Nombre,
                Orden = g.First().orden,
                Cuantos = admitidos.Count(a => a.Destinos.Any(d => d.Clave == g.Key)),
            })
            .OrderByDescending(o => o.Cuantos).ThenBy(o => o.Orden)
            .ToList();

        _formatos.Clear();
        foreach (var o in opciones)
            _formatos.Add(new OpcionFormato(o.Clave, o.Nombre, o.Cuantos, admitidos.Count) { Elegida = o.Clave == _clave });

        if (_formatos.All(f => f.Clave != _clave)) _clave = null;
    }

    private void Formato_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: OpcionFormato o }) _clave = o.Clave;
        Actualizar();
    }

    private void Destino_Changed(object sender, RoutedEventArgs e)
    {
        if (Carpeta != null) Carpeta.IsEnabled = OtraCarpeta.IsChecked == true;
        Actualizar();
    }

    private void ElegirCarpeta_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFolderDialog { Title = "Carpeta donde guardar lo convertido" };
        if (Directory.Exists(Carpeta.Text)) dlg.InitialDirectory = Carpeta.Text;
        if (dlg.ShowDialog(this) != true) return;
        Carpeta.Text = dlg.FolderName;
        OtraCarpeta.IsChecked = true;
    }

    // =====================================================================================
    // Convertir
    // =====================================================================================

    private async void Convertir_Click(object sender, RoutedEventArgs e)
    {
        if (_clave == null || _trabajando) return;

        string? carpetaFija = null;
        if (OtraCarpeta.IsChecked == true)
        {
            carpetaFija = Carpeta.Text.Trim();
            if (carpetaFija.Length == 0)
            {
                MessageBox.Show(this, "Elige la carpeta donde guardar el resultado.", "Convertidor",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Directory.CreateDirectory(carpetaFija);
        }

        var clave = _clave;
        var nombreFormato = _formatos.First(f => f.Clave == clave).Nombre;
        var tanda = _archivos.ToList();

        _trabajando = true;
        Progreso.Value = 0;
        Progreso.Maximum = tanda.Count;
        Progreso.Visibility = Visibility.Visible;
        Actualizar();

        int bien = 0, mal = 0, fuera = 0;

        // Office por COM exige un hilo STA. El de la ventana lo es, pero si convirtiera en él
        // la ventana se quedaría congelada; así que se abre un hilo STA aparte.
        await EjecutarEnSta(() =>
        {
            using var contexto = new ContextoConversion();
            foreach (var a in tanda)
            {
                var plan = Catalogo.Resolver(a.Ruta, clave);
                if (plan == null)
                {
                    fuera++;
                    Dispatcher.Invoke(() => a.Poner(EstadoArchivo.NoAdmitido, $"No se puede pasar a {nombreFormato}"));
                }
                else
                {
                    Dispatcher.Invoke(() => a.Poner(EstadoArchivo.Convirtiendo, "Convirtiendo…"));
                    try
                    {
                        var carpeta = carpetaFija ?? Path.GetDirectoryName(a.Ruta)!;
                        var salida = plan.Value.Conversor.Convertir(a.Ruta, plan.Value.Destino, carpeta, contexto);
                        bien++;
                        Dispatcher.Invoke(() =>
                        {
                            a.Resultado = salida;
                            a.Poner(EstadoArchivo.Hecho, "→ " + Path.GetFileName(salida));
                        });
                    }
                    catch (Exception ex)
                    {
                        mal++;
                        Dispatcher.Invoke(() => a.Poner(EstadoArchivo.Error, "Error: " + Explicar(ex)));
                    }
                }
                Dispatcher.Invoke(() => Progreso.Value++);
            }
        });

        _trabajando = false;
        Progreso.Visibility = Visibility.Collapsed;
        Actualizar();

        var partes = new List<string> { $"{bien} convertido{(bien == 1 ? "" : "s")}" };
        if (mal > 0) partes.Add($"{mal} con error");
        if (fuera > 0) partes.Add($"{fuera} sin ese formato");
        Resumen.Text = $"Terminado: {string.Join(", ", partes)}. Doble clic en un archivo para verlo en su carpeta.";
    }

    private static Task EjecutarEnSta(Action accion)
    {
        var tcs = new TaskCompletionSource();
        var hilo = new Thread(() =>
        {
            try { accion(); tcs.SetResult(); }
            catch (Exception ex) { tcs.SetException(ex); }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.IsBackground = true;
        hilo.Start();
        return tcs.Task;
    }

    private static string Explicar(Exception ex)
    {
        while (ex is TargetInvocationException { InnerException: not null }) ex = ex.InnerException;
        var texto = ex.Message.Replace("\r", " ").Replace("\n", " ").Trim();
        if (ex is IOException && texto.Contains("being used", StringComparison.OrdinalIgnoreCase))
            return "el archivo está abierto en otro programa";
        if (ex is COMException && texto.Length == 0) return "Office no ha podido abrirlo";
        return texto;
    }

    // =====================================================================================
    // Estado de la ventana
    // =====================================================================================

    private void Actualizar()
    {
        if (BotonConvertir == null) return; // durante InitializeComponent

        var hay = _archivos.Count > 0;
        Vacio.Visibility = hay ? Visibility.Collapsed : Visibility.Visible;
        Lista.Visibility = hay ? Visibility.Visible : Visibility.Collapsed;
        SinFormatos.Visibility = _formatos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        SinFormatos.Text = hay
            ? "Ninguno de estos archivos se puede convertir."
            : "Añade archivos para ver a qué formatos se pueden pasar.";

        BotonQuitar.IsEnabled = !_trabajando && Lista.SelectedItems.Count > 0;
        BotonRecortar.IsEnabled = Recortable != null;
        BotonVaciar.IsEnabled = !_trabajando && hay;
        Formatos.IsEnabled = !_trabajando;
        BotonConvertir.IsEnabled = !_trabajando && _clave != null && hay;
        BotonConvertir.Content = _trabajando ? "Convirtiendo…" : "Convertir";

        if (_trabajando) return;

        Resumen.Text = !hay ? ""
            : _clave == null ? $"{_archivos.Count} archivo{(_archivos.Count == 1 ? "" : "s")} · elige el formato de salida"
            : $"{_archivos.Count(a => a.Destinos.Any(d => d.Clave == _clave))} de {_archivos.Count} se pasarán a "
              + _formatos.First(f => f.Clave == _clave).Nombre;
    }
}
