using System.Windows.Controls;
using SigeGestor.App.ViewModels;

namespace SigeGestor.App.Views.Paginas;

public partial class InicioPage : UserControl
{
    public InicioPage(InicioViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;

        // La carga arranca sola al aparecer. Se hace aquí y no en el constructor del
        // ViewModel para no lanzar trabajo asíncrono desde un constructor.
        Loaded += async (_, _) => await vm.CargarAsync();
    }
}
