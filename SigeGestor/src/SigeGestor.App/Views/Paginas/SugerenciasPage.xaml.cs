using System.Windows;
using System.Windows.Controls;
using SigeGestor.App.ViewModels;

namespace SigeGestor.App.Views.Paginas;

/// <summary>
/// Lo que el equipo propone para la aplicación. Cada sugerencia es un fichero en
/// Sugerencias\ junto al ejecutable, así que la lista es la misma para los ocho.
/// </summary>
public partial class SugerenciasPage : UserControl
{
    private readonly SugerenciasViewModel _vm;

    public SugerenciasPage(SugerenciasViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;
    }

    /// <summary>
    /// Borrar pregunta antes, y por eso va por Click y no por comando: lo que se borra es lo
    /// que alguien se tomó la molestia de escribir y no hay de dónde recuperarlo.
    /// </summary>
    private void Borrar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: SugerenciaVm fila }) return;

        var respuesta = MessageBox.Show(
            Window.GetWindow(this),
            "¿Borrar esta sugerencia?" + System.Environment.NewLine + System.Environment.NewLine +
            fila.Titulo + System.Environment.NewLine + System.Environment.NewLine +
            "No se puede deshacer.",
            "Borrar sugerencia",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);

        if (respuesta != MessageBoxResult.Yes) return;

        _vm.BorrarConfirmado(fila);
    }
}
