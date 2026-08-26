using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SigeGestor.App.ViewModels;
using SigeGestor.App.Views.Dialogos;

namespace SigeGestor.App.Views.Paginas;

/// <summary>
/// Mantenimiento de las empresas. Es el RegistrarBD de ActualizaPrecios más lo que le faltaba:
/// ver la lista, editar, eliminar y probar la conexión.
/// </summary>
public partial class EmpresasPage : UserControl
{
    private readonly EmpresasViewModel _vm;

    public EmpresasPage(EmpresasViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;
    }

    public event EventHandler? VolverPedido;

    private void Volver_Click(object sender, RoutedEventArgs e) =>
        VolverPedido?.Invoke(this, EventArgs.Empty);

    private void Nueva_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new EditarEmpresaWindow { Owner = Window.GetWindow(this) };

        if (dialogo.ShowDialog() == true && dialogo.Resultado is not null)
        {
            _vm.Anadir(dialogo.Resultado);
        }
    }

    private void Editar_Click(object sender, RoutedEventArgs e) => Editar();

    private void Lista_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Doble clic en la cabecera o en el hueco no debe abrir nada.
        if (_vm.Seleccionada is not null) Editar();
    }

    private void Editar()
    {
        var actual = _vm.Seleccionada;
        if (actual is null) return;

        var dialogo = new EditarEmpresaWindow(actual.Empresa.Copiar())
        {
            Owner = Window.GetWindow(this)
        };

        if (dialogo.ShowDialog() == true && dialogo.Resultado is not null)
        {
            _vm.Reemplazar(actual, dialogo.Resultado);
        }
    }

    private void Eliminar_Click(object sender, RoutedEventArgs e)
    {
        var actual = _vm.Seleccionada;
        if (actual is null) return;

        // Se pregunta porque borra una línea del fichero que comparten los ocho, y no hay
        // deshacer. La copia .bak que deja el repositorio es el único margen.
        var respuesta = MessageBox.Show(
            Window.GetWindow(this),
            $"Se va a quitar «{actual.Nombre}» del fichero de empresas.\n\n" +
            "No se borra nada de la base de datos: solo deja de aparecer aquí y en los modelos " +
            "de impresión. ¿Seguir?",
            "Eliminar empresa",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (respuesta == MessageBoxResult.Yes) _vm.Quitar(actual);
    }

    private async void Probar_Click(object sender, RoutedEventArgs e) => await _vm.ProbarTodasAsync();

    private void Recargar_Click(object sender, RoutedEventArgs e) => _vm.Recargar();
}
