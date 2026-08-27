using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using SigeGestor.App.ViewModels;
using SigeGestor.App.Views.Paginas;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Operaciones;
using SigeGestor.Core.Registro;
using SigeGestor.Core.Servicios;
using SigeGestor.Core.Sesion;

namespace SigeGestor.App.Views;

/// <summary>
/// Ventana principal: barra lateral con las secciones, barra superior con la línea de entorno
/// y el área donde vive la página de la sección activa.
///
/// Las páginas se crean la primera vez que se entra en su sección y se guardan: así volver a
/// una sección conserva lo que hubiera escrito, y no se paga el coste de montarlas todas al
/// arrancar.
/// </summary>
public partial class ShellWindow : Window
{
    private readonly ShellViewModel _vm;
    private readonly Dictionary<Seccion, UserControl> _paginas = new();
    private readonly Usuario _usuario;
    private readonly RepositorioEntornos _entornos;
    private readonly RegistroEjecuciones _registro;
    private readonly IRepositorioEjecuciones _ejecuciones;
    private readonly int _diasRetencionLog;
    private readonly BuscadorViewModel _buscador = new();

    public ShellWindow(Usuario usuario,
                       RepositorioEntornos entornos,
                       IRepositorioEjecuciones ejecuciones,
                       RegistroEjecuciones registro,
                       int diasRetencionLog)
    {
        InitializeComponent();

        _usuario = usuario;
        _entornos = entornos;
        _registro = registro;
        _ejecuciones = ejecuciones;
        _diasRetencionLog = diasRetencionLog;

        // Se arranca en Replica: es el entorno de lectura, el que no bloquea a los usuarios
        // de SIGE. Cada operación elige el suyo cuando se abre; esto es solo el de la barra.
        EntornoBD? entorno = null;
        try
        {
            entorno = entornos.Obtener(ClaveEntorno.Replica);
        }
        catch (ConfiguracionNoDisponibleException)
        {
            // Sin Replica configurada se sigue adelante y la barra queda en neutro, en vez
            // de impedir el acceso a alguien que solo quiere consultar otra cosa.
        }

        SesionActual.EntornoActivo = entorno;

        _vm = new ShellViewModel(usuario, entorno);
        _vm.PropertyChanged += VmPropertyChanged;
        DataContext = _vm;

        MostrarSeccion(_vm.SeccionActual);

        // Ctrl+K a nivel de ventana. Con PreviewKeyDown y no KeyDown para que funcione aunque
        // el foco esté dentro de un cuadro de texto de una operación, que es lo normal.
        PreviewKeyDown += ShellPreviewKeyDown;
    }

    /// <summary>
    /// La campana lleva a Novedades. Antes era un botón sin Click: se pulsaba y no pasaba nada.
    /// Al llegar allí se marcan como leídas y el punto se apaga solo.
    /// </summary>
    private void Campana_Click(object sender, RoutedEventArgs e) => _vm.Ir(Seccion.Novedades);

    private void VmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ShellViewModel.SeccionActual))
        {
            MostrarSeccion(_vm.SeccionActual);
        }
    }

    private void MostrarSeccion(ItemNavegacion? item)
    {
        if (item is null) return;

        if (!_paginas.TryGetValue(item.Seccion, out var pagina))
        {
            pagina = CrearPagina(item);
            if (pagina is null) return;   // sección sin página: se ve el marcador del XAML
            _paginas[item.Seccion] = pagina;
        }

        AreaContenido.Content = pagina;
    }

    private UserControl? CrearPagina(ItemNavegacion item)
    {
        if (item.Seccion == Seccion.Inicio)
        {
            return new InicioPage(
                new InicioViewModel(_ejecuciones, _usuario) { DiasRetencion = _diasRetencionLog });
        }

        if (item.Seccion == Seccion.Ejecuciones)
        {
            return new EjecucionesPage(new EjecucionesViewModel(_ejecuciones, _usuario));
        }

        if (item.Seccion == Seccion.Novedades)
        {
            var novedades = new NovedadesViewModel(_usuario);

            // Al marcarlas leídas se apaga el número de la barra lateral sin reiniciar.
            novedades.Leido += (_, _) => _vm.RefrescarAvisos();

            return new NovedadesPage(novedades);
        }

        if (item.SeccionCatalogo is SeccionOperacion seccion)
        {
            var pagina = new SeccionPage(new SeccionViewModel(
                seccion,
                item.Titulo,
                DescripcionDe(seccion),
                item.ClaveIcono));

            pagina.OperacionElegida += (_, definicion) => AbrirOperacion(definicion, pagina);
            return pagina;
        }

        return null;
    }

    /// <summary>
    /// Abre el formulario de una operación por encima de su sección. No se guarda en caché:
    /// entrar de nuevo en una operación debe dar un formulario limpio, no lo que se dejó a
    /// medias la vez anterior.
    /// </summary>
    private void AbrirOperacion(DefinicionOperacion definicion, UserControl paginaSeccion)
    {
        // Lo que no cabe en el formulario generado tiene página propia. Son las dos rejillas de
        // mantenimiento de Ajustes; el resto de las 45 operaciones va por el camino de arriba.
        if (definicion.PaginaPropia.Length > 0)
        {
            var propia = CrearPaginaPropia(definicion, paginaSeccion);
            if (propia is not null)
            {
                AreaContenido.Content = propia;
                return;
            }
        }

        var pagina = new OperacionPage(
            new OperacionViewModel(definicion, _entornos), _registro, _usuario);
        pagina.VolverPedido += (_, _) => AreaContenido.Content = paginaSeccion;
        AreaContenido.Content = pagina;
    }

    /// <summary>
    /// Las páginas propias tampoco se guardan en caché: entrar de nuevo debe releer el fichero
    /// y la base, no mostrar lo que hubiera cargado la vez anterior.
    /// </summary>
    private UserControl? CrearPaginaPropia(DefinicionOperacion definicion, UserControl paginaSeccion)
    {
        switch (definicion.PaginaPropia)
        {
            case PaginasPropias.Empresas:
                {
                    var pagina = new EmpresasPage(new EmpresasViewModel());
                    pagina.VolverPedido += (_, _) => AreaContenido.Content = paginaSeccion;
                    return pagina;
                }

            case PaginasPropias.ModelosImpresion:
                {
                    var pagina = new ModelosImpresionPage(new ModelosImpresionViewModel());
                    pagina.VolverPedido += (_, _) => AreaContenido.Content = paginaSeccion;
                    return pagina;
                }

            default:
                return null;
        }
    }

    private static string DescripcionDe(SeccionOperacion seccion) => seccion switch
    {
        SeccionOperacion.Precios =>
            "Todo lo que toca precios de tarifa. Estas operaciones escriben en la base de datos, " +
            "así que se ejecutan contra Producción y Replica no se ofrece como destino.",
        SeccionOperacion.Contratos =>
            "Cambios en bloque sobre contratos: renovaciones, agentes, administradores y códigos.",
        SeccionOperacion.Productos =>
            "Asignación de productos a contratos, con su importe y su forma de aplicación.",
        SeccionOperacion.Consultas =>
            "Las consultas que se entregan en Excel. Abren contra Replica para no provocar " +
            "bloqueos a los usuarios de SIGE; puedes cambiar de entorno en cada una.",
        SeccionOperacion.Facturas =>
            "Descarga y organización de PDF de factura, y utilidades sobre los XML de facturación.",
        _ => "Mantenimiento de las tablas de apoyo y de la configuración de la aplicación."
    };

    protected override void OnClosed(EventArgs e)
    {
        _vm.PropertyChanged -= VmPropertyChanged;
        SesionActual.Cerrar();
        base.OnClosed(e);
    }

    // ============================================================
    // BUSCADOR
    // ============================================================

    private void ShellPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.K && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            AbrirBuscador();
            e.Handled = true;
        }
    }

    private void Buscar_Click(object sender, RoutedEventArgs e) => AbrirBuscador();

    private void AbrirBuscador()
    {
        _buscador.Abierto = true;
        CapaBuscador.DataContext = _buscador;
        CapaBuscador.Visibility = Visibility.Visible;

        // El foco hay que darlo cuando la capa ya es visible; si se hace antes, WPF lo
        // descarta porque el control todavía no está en el árbol visual.
        Dispatcher.BeginInvoke(new Action(() =>
        {
            CajaBuscar.Focus();
            CajaBuscar.SelectAll();
        }), DispatcherPriority.Input);
    }

    private void CerrarBuscador()
    {
        _buscador.Abierto = false;
        CapaBuscador.Visibility = Visibility.Collapsed;
    }

    private void CerrarBuscador_Click(object sender, MouseButtonEventArgs e) => CerrarBuscador();

    /// <summary>
    /// Las flechas y el Intro se atienden en la caja de texto, no en la lista: así se puede
    /// seguir escribiendo y moverse sin sacar las manos del teclado ni cambiar el foco.
    /// </summary>
    private void CajaBuscar_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                CerrarBuscador();
                e.Handled = true;
                break;

            case Key.Down:
                _buscador.Mover(1);
                DesplazarAlSeleccionado();
                e.Handled = true;
                break;

            case Key.Up:
                _buscador.Mover(-1);
                DesplazarAlSeleccionado();
                e.Handled = true;
                break;

            case Key.Enter:
                AbrirSeleccionado();
                e.Handled = true;
                break;
        }
    }

    private void DesplazarAlSeleccionado()
    {
        if (_buscador.Seleccionado is not null) ListaBuscar.ScrollIntoView(_buscador.Seleccionado);
    }

    private void ListaBuscar_Click(object sender, MouseButtonEventArgs e) => AbrirSeleccionado();

    /// <summary>
    /// Abre la operación elegida: primero lleva a su sección —así la barra lateral queda
    /// marcada y el botón «volver» tiene a dónde ir— y luego abre el formulario encima.
    /// </summary>
    private void AbrirSeleccionado()
    {
        var elegido = _buscador.Seleccionado;
        if (elegido is null) return;

        CerrarBuscador();

        var definicion = elegido.Definicion;

        var item = _vm.Navegacion.FirstOrDefault(i => i.SeccionCatalogo == definicion.Seccion);
        if (item is null) return;

        _vm.SeccionActual = item;   // esto crea o recupera la página de la sección

        if (_paginas.TryGetValue(item.Seccion, out var paginaSeccion))
        {
            AbrirOperacion(definicion, paginaSeccion);
        }
    }
}
