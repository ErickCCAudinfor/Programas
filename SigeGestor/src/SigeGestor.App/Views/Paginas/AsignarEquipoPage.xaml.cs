using System;
using System.Windows;
using System.Windows.Controls;
using SigeGestor.App.ViewModels;

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

        var respuesta = MessageBox.Show(
            Window.GetWindow(this),
            _vm.ResumenParaConfirmar + Environment.NewLine + Environment.NewLine +
            "Solo se tocan las lecturas que no tenían equipo." + Environment.NewLine +
            "¿Seguir?",
            "Asignar nº de equipo",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (respuesta != MessageBoxResult.Yes) return;

        await _vm.GuardarConfirmadoAsync();
    }
}
