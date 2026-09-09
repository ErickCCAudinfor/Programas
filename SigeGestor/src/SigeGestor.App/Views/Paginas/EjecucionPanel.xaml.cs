using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

    // ============================================================
    // ABRIR DONDE HA QUEDADO LO GENERADO
    // ============================================================

    /// <summary>
    /// Abre el explorador en la carpeta. Con un solo fichero se abre con él ya seleccionado,
    /// que ahorra buscarlo entre los de las ejecuciones anteriores —todas van a la misma
    /// carpeta y llevan la hora en el nombre—.
    ///
    /// La ruta se pasa entre comillas: ConsultasBO cuelga del escritorio y ahí hay espacios en
    /// cuanto el perfil los tiene.
    /// </summary>
    private void AbrirCarpeta_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: SalidaVm salida }) return;

        try
        {
            if (salida.Seleccionar.Length > 0 && File.Exists(salida.Seleccionar))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{salida.Seleccionar}\"")
                {
                    UseShellExecute = true
                });
                return;
            }

            if (!Directory.Exists(salida.Carpeta))
            {
                MessageBox.Show(
                    Window.GetWindow(this),
                    "La carpeta ya no está:" + Environment.NewLine + Environment.NewLine +
                    salida.Carpeta,
                    "Abrir carpeta",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Process.Start(new ProcessStartInfo(salida.Carpeta) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                Window.GetWindow(this),
                "No se ha podido abrir la carpeta." + Environment.NewLine + Environment.NewLine +
                salida.Carpeta + Environment.NewLine + Environment.NewLine +
                $"Detalle: {ex.Message}",
                "Abrir carpeta",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    // ============================================================
    // COPIAR LAS QUE NO SALIERON BIEN
    // ============================================================

    private void CopiarEntradas_Click(object sender, RoutedEventArgs e) =>
        Copiar(_vm.ParaPegar, "solo las entradas");

    private void CopiarConMotivo_Click(object sender, RoutedEventArgs e) =>
        Copiar(_vm.ParaExcel, "con el motivo");

    /// <summary>
    /// Copia al portapapeles.
    ///
    /// SetDataObject con copy:true y no SetText: así el contenido se vuelca al portapapeles de
    /// Windows y sigue estando cuando se cierra la aplicación. Con SetText, cerrar SigeGestor
    /// antes de pegar deja el portapapeles vacío, que es exactamente lo que se va a hacer:
    /// copiar el listado y cerrar.
    ///
    /// Y va en try: el portapapeles es un recurso compartido y otro proceso puede tenerlo
    /// abierto un instante, lo que hace saltar COMException. Con avisar basta; no merece
    /// tumbar la pantalla del resultado.
    /// </summary>
    private void Copiar(string texto, string queEs)
    {
        if (string.IsNullOrEmpty(texto)) return;

        try
        {
            Clipboard.SetDataObject(texto, copy: true);
            _vm.ConfirmarCopiado(_vm.Incidencias.Count, queEs);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                Window.GetWindow(this),
                "No se ha podido copiar al portapapeles. Suele ser que otro programa lo tiene " +
                "ocupado un momento; prueba otra vez." + Environment.NewLine + Environment.NewLine +
                $"Detalle: {ex.Message}",
                "Copiar",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    /// <summary>Se suelta el enganche al ViewModel cuando el panel deja de usarse.</summary>
    public void Desconectar() => _vm.PropertyChanged -= VmPropertyChanged;
}
