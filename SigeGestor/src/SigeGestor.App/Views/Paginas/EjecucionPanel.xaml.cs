using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SigeGestor.App.ViewModels;

namespace SigeGestor.App.Views.Paginas;

public partial class EjecucionPanel : UserControl
{
    private readonly EjecucionViewModel _vm;

    public EjecucionPanel(EjecucionViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;

        // El ancho de la barra se calcula a mano: enlazarlo exigiría un MultiBinding con el
        // ancho del contenedor, y así además se recalcula al redimensionar la ventana.
        _vm.PropertyChanged += VmPropertyChanged;
        BarraProgreso.SizeChanged += (_, _) => AjustarBarra();
        SizeChanged += (_, _) => AjustarBarra();
    }

    public event EventHandler? VolverPedido;

    public event EventHandler<IReadOnlyList<string>>? ReintentarPedido;

    private void VmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EjecucionViewModel.Fraccion))
        {
            AjustarBarra();
        }
    }

    private void AjustarBarra()
    {
        if (BarraProgreso.Parent is not FrameworkElement contenedor) return;

        var disponible = contenedor.ActualWidth;
        if (disponible <= 0) return;

        BarraProgreso.Width = Math.Max(0, Math.Min(disponible, disponible * _vm.Fraccion));
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e) => _vm.Cancelar();

    private void Volver_Click(object sender, RoutedEventArgs e) => VolverPedido?.Invoke(this, EventArgs.Empty);

    private void Reintentar_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.Fallidas.Count > 0)
        {
            ReintentarPedido?.Invoke(this, _vm.Fallidas);
        }
    }

    /// <summary>Se suelta el enganche al ViewModel cuando el panel deja de usarse.</summary>
    public void Desconectar() => _vm.PropertyChanged -= VmPropertyChanged;
}
