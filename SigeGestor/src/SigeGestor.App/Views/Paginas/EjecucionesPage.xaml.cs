using System.Windows;
using System.Windows.Controls;
using SigeGestor.App.ViewModels;

namespace SigeGestor.App.Views.Paginas;

/// <summary>
/// El histórico de ejecuciones del equipo. La entrada de la barra lateral existía desde el
/// principio y no hacía nada al pulsarla.
/// </summary>
public partial class EjecucionesPage : UserControl
{
    private readonly EjecucionesViewModel _vm;

    public EjecucionesPage(EjecucionesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;

        // Se carga al mostrarse. La página se cachea, así que al volver a entrar se recarga
        // sola: es lo que se espera de un histórico, que esté al día sin pedirlo.
        Loaded += async (_, _) => await _vm.CargarAsync();
    }

    private void Limpiar_Click(object sender, RoutedEventArgs e) => _vm.Limpiar();

    private async void Recargar_Click(object sender, RoutedEventArgs e) => await _vm.CargarAsync();
}
