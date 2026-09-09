using System;
using System.Windows;
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

    // ============================================================
    // LOS DOS BOTONES DE LA CABECERA
    // ============================================================
    //
    // Estaban puestos y no hacían nada: se pulsaban y no pasaba nada. Se resuelven por evento,
    // como el resto de las páginas, porque quien sabe navegar es el armazón y no la página.

    /// <summary>Llevar al histórico de ejecuciones.</summary>
    public event EventHandler? VerEjecucionesPedido;

    /// <summary>
    /// Empezar una ejecución nueva. Abre la paleta de búsqueda, que es el camino más corto a
    /// cualquiera de las 46 operaciones: escribir el nombre. Lo mismo que Ctrl+K.
    /// </summary>
    public event EventHandler? NuevaEjecucionPedida;

    private void VerEjecuciones_Click(object sender, RoutedEventArgs e) =>
        VerEjecucionesPedido?.Invoke(this, EventArgs.Empty);

    private void NuevaEjecucion_Click(object sender, RoutedEventArgs e) =>
        NuevaEjecucionPedida?.Invoke(this, EventArgs.Empty);
}
