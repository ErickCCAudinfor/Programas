using System;
using System.Windows;
using System.Windows.Controls;
using SigeGestor.App.ViewModels;
using SigeGestor.App.Views.Dialogos;

namespace SigeGestor.App.Views.Paginas;

/// <summary>
/// Asignar el equipo de medida a las lecturas de una prefactura.
///
/// Tiene página propia y no formulario generado porque hace falta una rejilla donde se elige
/// un equipo POR CADA lectura; eso no se puede describir con campos declarados.
/// </summary>
public partial class AsignarEquipoPage : UserControl
{
    private readonly AsignarEquipoViewModel _vm;

    public AsignarEquipoPage(AsignarEquipoViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;
    }

    public event EventHandler? VolverPedido;

    private void Volver_Click(object sender, RoutedEventArgs e) =>
        VolverPedido?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Pregunta antes de escribir. Esto toca la tabla de lecturas de Producción y no hay
    /// ningún paso de «ver qué pasaría» como en los scripts: lo que se guarda, se guarda.
    /// </summary>
    private async void Guardar_Click(object sender, RoutedEventArgs e)
    {
        if (!_vm.PuedeGuardar) return;

        var adelante = PreguntaWindow.Preguntar(
            Window.GetWindow(this),
            "Asignar nº de equipo",
            _vm.ResumenParaConfirmar + Environment.NewLine +
            "Solo se tocan las lecturas que no tenían equipo.",
            siTexto: "Asignar el equipo");

        if (!adelante) return;

        await _vm.GuardarConfirmadoAsync();
    }
}
