using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SigeGestor.App.ViewModels;
using SigeGestor.Core;
using SigeGestor.Core.Modelos;

namespace SigeGestor.App.Views.Dialogos;

/// <summary>
/// Alta y edición de un modelo de impresión. Es el EditarModeloImpresionForm de ActualizaPrecios.
///
/// UNA DIFERENCIA: allí el binario del modelo se descargaba SIEMPRE al abrir la ventana, para
/// mostrar su tamaño en una etiqueta. Son varios megas por fila y contra una base remota se
/// notaba. Aquí solo se pide si se va a cambiar el fichero, y mientras se conserva el que hay.
/// </summary>
public partial class EditarModeloWindow : Window
{
    private readonly ModeloDeImpresion? _original;
    private byte[]? _reportNuevo;

    public EditarModeloWindow(IReadOnlyList<TipoModeloVm> tipos,
                              string nombreEmpresa,
                              ModeloDeImpresion? original = null)
    {
        InitializeComponent();

        _original = original;
        CajaTipo.ItemsSource = tipos;

        if (original is null)
        {
            Subtitulo.Text = $"Se creará en {nombreEmpresa}.";
            CajaEntorno.SelectedIndex = 0;
            CajaTipo.SelectedItem = tipos.FirstOrDefault(t => t.Codigo == 1);   // Factura
        }
        else
        {
            Titulo.Text = $"Modelo {original.IdModeloDeImpresion}";
            Title = $"Modelo · {nombreEmpresa}";
            Subtitulo.Text = $"Se modificará en {nombreEmpresa}.";

            CajaDescripcion.Text = original.DescripcionModeloDeImpresion ?? "";
            CajaClase.Text = original.ClassName ?? "";
            CajaReport.Text = original.RptFileName ?? "";

            CajaEntorno.SelectedIndex = original.Entorno == "G2" ? 1 : 0;
            CajaTipo.SelectedItem = tipos.FirstOrDefault(t => t.Codigo == original.CodigoTipoModeloDeImpresion);

            AvisoReport.Visibility = Visibility.Visible;
        }

        ContentRendered += (_, _) => CajaDescripcion.Focus();
        Revisar();
    }

    /// <summary>El modelo resultante. Solo tiene valor si se ha pulsado Guardar.</summary>
    public ModeloDeImpresion? Resultado { get; private set; }

    /// <summary>True si hay que insertar y no actualizar.</summary>
    public bool EsNuevo => _original is null;

    private void Revisar(object sender, RoutedEventArgs e) => Revisar();

    private void Revisar(object sender, SelectionChangedEventArgs e) => Revisar();

    private void Revisar()
    {
        if (BotonGuardar is null) return;

        var problema = PrimerProblema();
        TextoProblema.Text = problema;
        TextoProblema.Visibility = problema.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
        BotonGuardar.IsEnabled = problema.Length == 0;
    }

    private string PrimerProblema()
    {
        if (string.IsNullOrWhiteSpace(CajaDescripcion.Text)) return "Falta la descripción.";
        if (CajaEntorno.SelectedItem is null) return "Elige el entorno.";
        if (CajaTipo.SelectedItem is null) return "Elige el tipo.";

        // En un alta el report es obligatorio: una fila sin binario no imprime nada, y es
        // justo lo que la comprobación masiva marca como «le falta».
        if (_original is null && _reportNuevo is null) return "Elige el fichero .rpt.";

        return string.Empty;
    }

    private void ElegirReport_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new OpenFileDialog
        {
            Title = "Seleccionar modelo de impresión",
            Filter = "Crystal Reports (*.rpt)|*.rpt|Todos los ficheros (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialogo.ShowDialog() != true) return;

        try
        {
            var bytes = File.ReadAllBytes(dialogo.FileName);

            if (bytes.Length < 10)
            {
                TextoProblema.Text = "Ese fichero está vacío o es demasiado pequeño para ser un report.";
                TextoProblema.Visibility = Visibility.Visible;
                return;
            }

            _reportNuevo = bytes;
            CajaReport.Text = $"{Path.GetFileName(dialogo.FileName)} · {bytes.Length / 1024.0:N0} KB";
        }
        catch (Exception ex)
        {
            TextoProblema.Text = $"No se ha podido leer el fichero: {ex.Message}";
            TextoProblema.Visibility = Visibility.Visible;
            return;
        }

        Revisar();
    }

    /// <summary>
    /// El nombre de fichero que se guarda es el del .rpt elegido; si no se ha elegido ninguno,
    /// el que ya tenía. No se guarda el «nombre · KB» que se muestra en la caja.
    /// </summary>
    private void Guardar_Click(object sender, RoutedEventArgs e)
    {
        var entorno = (CajaEntorno.SelectedItem as ComboBoxItem)?.Tag as string ?? "G1";
        var tipo = (TipoModeloVm)CajaTipo.SelectedItem;

        var nombreRpt = _reportNuevo is not null
            ? CajaReport.Text.Split('·')[0].Trim()
            : _original?.RptFileName ?? "";

        Resultado = new ModeloDeImpresion
        {
            IdModeloDeImpresion = _original?.IdModeloDeImpresion ?? 0,
            Entorno = entorno,
            DescripcionModeloDeImpresion = CajaDescripcion.Text.Trim(),
            CodigoTipoModeloDeImpresion = tipo.Codigo,
            ClassName = CajaClase.Text.Trim(),
            RptFileName = nombreRpt,
            Modelo = _reportNuevo
        };

        DialogResult = true;
        Close();
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
