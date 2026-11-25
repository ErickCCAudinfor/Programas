using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using SigeGestor.App.ViewModels;
using SigeGestor.App.Views.Dialogos;
using SigeGestor.Core.Configuracion;

namespace SigeGestor.App.Views.Paginas;

/// <summary>
/// Mantenimiento de los modelos de impresión. Junta ModeloImpresionForm,
/// EditarModeloImpresionForm y AnadirMasivoEmpresaForm de ActualizaPrecios en una pantalla.
/// </summary>
public partial class ModelosImpresionPage : UserControl
{
    private readonly ModelosImpresionViewModel _vm;

    public ModelosImpresionPage(ModelosImpresionViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;
    }

    public event EventHandler? VolverPedido;

    private void Volver_Click(object sender, RoutedEventArgs e) =>
        VolverPedido?.Invoke(this, EventArgs.Empty);

    // ============================================================
    // EMPRESAS
    // ============================================================

    private void MarcarTodas_Click(object sender, RoutedEventArgs e) => _vm.MarcarTodas(true);

    private void MarcarNinguna_Click(object sender, RoutedEventArgs e) => _vm.MarcarTodas(false);

    private void EntornoMasivo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_vm is null) return;
        _vm.EntornoMasivo = (EntornoMasivo.SelectedItem as ComboBoxItem)?.Tag as string ?? "G1";
    }

    // ============================================================
    // MODELOS DE LA EMPRESA ACTIVA
    // ============================================================

    private void NuevoModelo_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.EmpresaActiva is null) return;

        var dialogo = new EditarModeloWindow(_vm.Tipos, _vm.EmpresaActiva.Nombre)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialogo.ShowDialog() == true && dialogo.Resultado is not null)
        {
            Guardar(dialogo);
        }
    }

    private void EditarModelo_Click(object sender, RoutedEventArgs e) => Editar();

    private void ListaModelos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_vm.ModeloSeleccionado is not null) Editar();
    }

    private void Editar()
    {
        if (_vm.EmpresaActiva is null || _vm.ModeloSeleccionado is null) return;

        var dialogo = new EditarModeloWindow(
            _vm.Tipos, _vm.EmpresaActiva.Nombre, _vm.ModeloSeleccionado.Modelo)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialogo.ShowDialog() == true && dialogo.Resultado is not null)
        {
            Guardar(dialogo);
        }
    }

    private async void Guardar(EditarModeloWindow dialogo)
    {
        var problema = await _vm.GuardarAsync(dialogo.Resultado!, dialogo.EsNuevo);

        if (problema.Length > 0)
        {
            PreguntaWindow.Avisar(Window.GetWindow(this), "No se ha podido guardar",
                                  problema, TonoDialogo.Cuidado);
        }
    }

    private async void BorrarModelo_Click(object sender, RoutedEventArgs e)
    {
        var modelo = _vm.ModeloSeleccionado;
        if (modelo is null || _vm.EmpresaActiva is null) return;

        // Se borra una fila de la base de un cliente y no hay deshacer: se pregunta con el
        // nombre de la empresa delante, que es el error fácil de cometer aquí.
        var borrar = PreguntaWindow.Preguntar(
            Window.GetWindow(this),
            $"Borrar el modelo {modelo.Id} de {_vm.EmpresaActiva.Nombre}",
            $"«{modelo.Descripcion}»\n\n" +
            "Si algún contrato o factura lo tiene asignado, dejará de imprimir. No hay deshacer.",
            siTexto: "Borrarlo",
            tono: TonoDialogo.Destructivo);

        if (!borrar) return;

        var problema = await _vm.BorrarAsync(modelo.Id);
        if (problema.Length > 0)
        {
            PreguntaWindow.Avisar(Window.GetWindow(this), "No se ha podido borrar",
                                  problema, TonoDialogo.Cuidado);
        }
    }

    /// <summary>
    /// Baja el .rpt de un modelo a disco. No estaba en ActualizaPrecios y hace falta: para
    /// copiar un modelo de una empresa a otra había que tener el fichero original a mano.
    /// </summary>
    private async void DescargarModelo_Click(object sender, RoutedEventArgs e)
    {
        var modelo = _vm.ModeloSeleccionado;
        if (modelo is null) return;

        var nombre = string.IsNullOrWhiteSpace(modelo.RptFileName)
            ? $"Modelo_{modelo.Id}.rpt"
            : modelo.RptFileName;

        var dialogo = new SaveFileDialog
        {
            Title = "Guardar el report",
            FileName = nombre,
            Filter = "Crystal Reports (*.rpt)|*.rpt|Todos los ficheros (*.*)|*.*",
            InitialDirectory = RutasSalida.Asegurar("Modelos")
        };

        if (dialogo.ShowDialog() != true) return;

        try
        {
            var bytes = await _vm.BinarioAsync(modelo.Id);

            if (bytes is null || bytes.Length == 0)
            {
                PreguntaWindow.Avisar(Window.GetWindow(this),
                    "Nada que guardar",
                    "Ese modelo no tiene report guardado en la base.");
                return;
            }

            File.WriteAllBytes(dialogo.FileName, bytes);
        }
        catch (Exception ex)
        {
            PreguntaWindow.Avisar(Window.GetWindow(this), "No se ha podido guardar",
                                  ex.Message, TonoDialogo.Cuidado);
        }
    }

    // ============================================================
    // MASIVO
    // ============================================================

    private void ElegirReportMasivo_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new OpenFileDialog
        {
            Title = "Seleccionar modelo de impresión",
            Filter = "Crystal Reports (*.rpt)|*.rpt|Todos los ficheros (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialogo.ShowDialog() == true) _vm.CargarReport(dialogo.FileName);
    }

    private async void Subir_Click(object sender, RoutedEventArgs e)
    {
        if (!Confirmar("subir el modelo a")) return;
        await _vm.SubirAsync();
    }

    private async void Actualizar_Click(object sender, RoutedEventArgs e)
    {
        if (!Confirmar("actualizar el modelo en")) return;
        await _vm.ActualizarAsync();
    }

    private async void Comprobar_Click(object sender, RoutedEventArgs e) => await _vm.ComprobarAsync();

    private void Cancelar_Click(object sender, RoutedEventArgs e) => _vm.Cancelar();

    /// <summary>
    /// Se confirma antes de escribir en varias bases de clientes. Comprobar no pregunta: no
    /// escribe nada.
    /// </summary>
    private bool Confirmar(string queSeVaAHacer)
    {
        var elegidas = _vm.Elegidas;

        var lista = string.Join("\n", elegidas.Select(d => $"  · {d.Nombre}"));

        var plural = elegidas.Count == 1 ? "empresa" : "empresas";

        return PreguntaWindow.Preguntar(
            Window.GetWindow(this),
            $"Se va a {queSeVaAHacer} {elegidas.Count} {plural}",
            $"{lista}\n\n" +
            "Son bases de datos de clientes distintas y el cambio no se deshace solo.",
            siTexto: "Seguir");
    }
}
