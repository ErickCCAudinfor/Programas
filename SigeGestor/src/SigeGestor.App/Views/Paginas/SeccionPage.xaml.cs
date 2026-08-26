using System;
using System.Windows.Controls;
using SigeGestor.App.ViewModels;
using SigeGestor.Core.Operaciones;

namespace SigeGestor.App.Views.Paginas;

public partial class SeccionPage : UserControl
{
    public SeccionPage(SeccionViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        Lista.SelectionChanged += ListaSelectionChanged;
    }

    /// <summary>Se ha elegido una operación de la lista.</summary>
    public event EventHandler<DefinicionOperacion>? OperacionElegida;

    private void ListaSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is not OperacionItemVm item) return;

        // Se deselecciona para que al volver de la operación la fila no quede marcada y se
        // pueda entrar otra vez en la misma.
        Lista.SelectedItem = null;

        OperacionElegida?.Invoke(this, item.Definicion);
    }
}
