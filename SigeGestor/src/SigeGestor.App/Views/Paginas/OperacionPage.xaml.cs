using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SigeGestor.App.ViewModels;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Registro;

namespace SigeGestor.App.Views.Paginas;

public partial class OperacionPage : UserControl
{
    private readonly OperacionViewModel _vm;
    private readonly RegistroEjecuciones _registro;
    private readonly Usuario _usuario;
    private EjecucionPanel? _panel;

    public OperacionPage(OperacionViewModel vm, RegistroEjecuciones registro, Usuario usuario)
    {
        InitializeComponent();
        _vm = vm;
        _registro = registro;
        _usuario = usuario;
        DataContext = vm;

        // Se valida al abrir para que el mensaje de lo que falta esté desde el principio,
        // en vez de aparecer solo después de tocar algo. Y se cargan las listas de los
        // campos de selección contra el entorno que trae la operación.
        Loaded += async (_, _) =>
        {
            _vm.RevisarValidez();
            await _vm.CargarCamposAsync();
        };
    }

    /// <summary>Vuelta a la página de la sección.</summary>
    public event EventHandler? VolverPedido;

    /// <summary>
    /// Se ha lanzado una ejecución. Lo escucha el armazón para poder decir en la cabecera que
    /// hay algo en marcha desde cualquier pantalla, y para guardar esta página y poder volver.
    ///
    /// Se avisa por evento y no pasando el registro al constructor para no acoplar la página a
    /// él: es el mismo camino que ya usan VolverPedido y ReintentarPedido.
    /// </summary>
    public event EventHandler<TareaEnCurso>? EjecucionLanzada;

    /// <summary>
    /// Se ha vuelto al formulario, así que esa ejecución ya se ha visto y sale del indicador.
    /// </summary>
    public event EventHandler? EjecucionVista;

    private void Volver_Click(object sender, RoutedEventArgs e) => VolverPedido?.Invoke(this, EventArgs.Empty);

    private void Limpiar_Click(object sender, RoutedEventArgs e) => _vm.Limpiar();

    // ============================================================
    // EJECUCIÓN
    // ============================================================

    private async void Ejecutar_Click(object sender, RoutedEventArgs e) => await LanzarAsync(null);

    private async System.Threading.Tasks.Task LanzarAsync(IReadOnlyList<string>? soloEstas)
    {
        if (!_vm.PuedeEjecutar) return;

        var contexto = _vm.ConstruirContexto(soloEstas);

        var ejecucion = new EjecucionViewModel(_registro, _usuario);
        _panel = new EjecucionPanel(ejecucion);
        _panel.VolverPedido += (_, _) => MostrarFormulario();
        _panel.ReintentarPedido += async (_, fallidas) => await LanzarAsync(fallidas);

        AreaEjecucion.Content = _panel;
        AreaEjecucion.Visibility = Visibility.Visible;
        Formulario.Visibility = Visibility.Collapsed;

        // Se avisa ANTES de arrancar, no después: el await de abajo no vuelve hasta que la
        // ejecución acaba, así que avisando después el indicador solo aparecería cuando ya no
        // hiciera falta, que es justo lo contrario de lo que se necesita.
        EjecucionLanzada?.Invoke(this, new TareaEnCurso
        {
            Operacion = _vm.Titulo,
            Grupo = _vm.Definicion.Seccion.ToString(),
            Entorno = contexto.Entorno.Clave,
            Ejecucion = ejecucion,
            Pagina = this
        });

        await ejecucion.EjecutarAsync(contexto);
    }

    private void MostrarFormulario()
    {
        _panel?.Desconectar();
        _panel = null;

        // Volver al formulario es haber visto el resultado: fuera del indicador.
        EjecucionVista?.Invoke(this, EventArgs.Empty);

        AreaEjecucion.Content = null;
        AreaEjecucion.Visibility = Visibility.Collapsed;
        Formulario.Visibility = Visibility.Visible;
    }

    // ============================================================
    // SELECTORES DE FICHERO
    // ============================================================

    private void ElegirExcel_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new OpenFileDialog
        {
            Title = "Elegir el fichero de Excel",
            Filter = "Libros de Excel (*.xlsx;*.xlsm;*.xls)|*.xlsx;*.xlsm;*.xls|Todos los ficheros (*.*)|*.*",
            CheckFileExists = true
        };

        if (!string.IsNullOrWhiteSpace(_vm.RutaExcel))
        {
            var carpeta = Path.GetDirectoryName(_vm.RutaExcel);
            if (Directory.Exists(carpeta)) dialogo.InitialDirectory = carpeta;
        }

        if (dialogo.ShowDialog() == true)
        {
            _vm.RutaExcel = dialogo.FileName;
        }
    }

    /// <summary>
    /// Selector de un campo de tipo Fichero declarado en el catálogo. El campo llega en el Tag
    /// del botón, que es lo que permite que el ItemsControl pinte varios sin código por campo.
    /// </summary>
    private void ElegirFicheroCampo_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button boton || boton.Tag is not CampoVm campo) return;

        var dialogo = new OpenFileDialog
        {
            Title = campo.Etiqueta,
            Filter = string.IsNullOrWhiteSpace(campo.Definicion.FiltroFichero)
                ? "Todos los ficheros (*.*)|*.*"
                : campo.Definicion.FiltroFichero,
            CheckFileExists = true
        };

        if (!string.IsNullOrWhiteSpace(campo.Texto))
        {
            var carpeta = Path.GetDirectoryName(campo.Texto);
            if (Directory.Exists(carpeta)) dialogo.InitialDirectory = carpeta;
        }

        if (dialogo.ShowDialog() == true)
        {
            campo.Texto = dialogo.FileName;
        }
    }

    private void ElegirCarpeta_Click(object sender, RoutedEventArgs e)
    {
        // OpenFolderDialog es nativo desde .NET 8; antes había que tirar de WinForms o de
        // la Windows API Code Pack.
        var dialogo = new OpenFolderDialog
        {
            Title = "Elegir la carpeta de destino"
        };

        if (Directory.Exists(_vm.CarpetaDestino))
        {
            dialogo.InitialDirectory = _vm.CarpetaDestino;
        }

        if (dialogo.ShowDialog() == true)
        {
            _vm.CarpetaDestino = dialogo.FolderName;
        }
    }
}
