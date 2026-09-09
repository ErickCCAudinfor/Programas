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

        _vm.CancelacionPedida += PreguntarYCancelar;
    }

    /// <summary>
    /// Pregunta antes de cancelar.
    ///
    /// SE PREGUNTA, al contrario que en el botón del panel de la ejecución: allí estás dentro,
    /// mirando cómo avanza y con el botón grande; aquí es un icono pequeño en una fila de una
    /// tabla, junto a otras filas, y un clic de más tiraría una consulta de cuarenta minutos.
    ///
    /// El mensaje dice por dónde va y qué pasa con lo hecho, que es lo que hace falta para
    /// decidir: en una operación de escritura, lo ya escrito se queda escrito.
    /// </summary>
    private void PreguntarYCancelar(object? emisor, EjecucionVm fila)
    {
        var respuesta = MessageBox.Show(
            Window.GetWindow(this),
            $"«{fila.Titulo}» lleva {fila.Resultado}." + Environment.NewLine + Environment.NewLine +
            "Si la cancelas se para donde esté. Lo que ya haya procesado se queda como está: " +
            "no se deshace." + Environment.NewLine + Environment.NewLine +
            "¿Cancelarla?",
            "Cancelar la ejecución",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (respuesta == MessageBoxResult.Yes) fila.CancelarAhora();
    }

    private void Limpiar_Click(object sender, RoutedEventArgs e) => _vm.Limpiar();

    private async void Recargar_Click(object sender, RoutedEventArgs e) => await _vm.CargarAsync();
}
