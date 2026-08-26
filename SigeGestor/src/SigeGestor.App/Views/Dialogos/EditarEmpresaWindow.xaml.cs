using System;
using System.Windows;
using System.Windows.Media;
using SigeGestor.App.ViewModels;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Seguridad;

namespace SigeGestor.App.Views.Dialogos;

/// <summary>
/// Alta y edición de una empresa. Es el RegistrarBD de ActualizaPrecios, que solo daba de alta.
///
/// Dos cosas que aquel no hacía: se puede probar la conexión antes de guardar, y al editar se
/// puede dejar la contraseña en blanco para conservar la que ya está cifrada en el fichero —lo
/// contrario obligaría a descifrarla y mostrarla en pantalla, que no hace falta—.
/// </summary>
public partial class EditarEmpresaWindow : Window
{
    private readonly RepositorioEmpresas _repositorio = new();
    private readonly EmpresaBD? _original;

    public EditarEmpresaWindow(EmpresaBD? original = null)
    {
        InitializeComponent();

        _original = original;

        if (original is not null)
        {
            Titulo.Text = "Editar empresa";
            Title = $"Empresa · {original.Nombre}";

            CajaNombre.Text = original.Nombre;
            CajaServidor.Text = original.Servidor;
            CajaBase.Text = original.BaseDatos;
            CajaVpn.IsChecked = original.VPN;

            try { CajaUsuario.Text = Cifrado.Descifrar(original.Usuario); }
            catch { CajaUsuario.Text = string.Empty; }

            AvisoClave.Visibility = Visibility.Visible;
        }

        // El foco al primer campo hay que ponerlo cuando la ventana ya está pintada: en el
        // constructor y en Loaded todavía se lo lleva el primer control que se enfoque solo.
        ContentRendered += (_, _) => CajaNombre.Focus();

        Revisar(this, new RoutedEventArgs());
    }

    /// <summary>La empresa resultante. Solo tiene valor si se ha pulsado Guardar.</summary>
    public EmpresaBD? Resultado { get; private set; }

    // ============================================================
    // VALIDACIÓN
    // ============================================================

    private void Revisar(object sender, RoutedEventArgs e) => Revisar();

    private void RevisarClave(object sender, RoutedEventArgs e) => Revisar();

    private void Revisar()
    {
        // Al construir, los controles del XAML aún pueden ser null cuando dispara TextChanged.
        if (BotonGuardar is null) return;

        var problema = PrimerProblema();

        TextoProblema.Text = problema;
        TextoProblema.Visibility = problema.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
        BotonGuardar.IsEnabled = problema.Length == 0;
        BotonProbar.IsEnabled = problema.Length == 0;
    }

    private string PrimerProblema()
    {
        if (string.IsNullOrWhiteSpace(CajaNombre.Text)) return "Falta el nombre.";
        if (string.IsNullOrWhiteSpace(CajaServidor.Text)) return "Falta el servidor.";
        if (string.IsNullOrWhiteSpace(CajaBase.Text)) return "Falta la base de datos.";
        if (string.IsNullOrWhiteSpace(CajaUsuario.Text)) return "Falta el usuario.";

        // En un alta la contraseña es obligatoria; al editar, en blanco significa «la de antes».
        if (_original is null && CajaClave.Password.Length == 0) return "Falta la contraseña.";

        return string.Empty;
    }

    /// <summary>
    /// Monta la empresa con lo escrito. La contraseña se cifra aquí; en blanco al editar se
    /// arrastra la que ya estaba, todavía cifrada, sin descifrarla en ningún momento.
    /// </summary>
    private EmpresaBD Componer()
    {
        var clave = CajaClave.Password.Length > 0
            ? Cifrado.Cifrar(CajaClave.Password)
            : _original?.Password ?? string.Empty;

        return new EmpresaBD
        {
            Nombre = CajaNombre.Text.Trim(),
            Servidor = CajaServidor.Text.Trim(),
            BaseDatos = CajaBase.Text.Trim(),
            Usuario = Cifrado.Cifrar(CajaUsuario.Text.Trim()),
            Password = clave,
            VPN = CajaVpn.IsChecked == true
        };
    }

    // ============================================================
    // ACCIONES
    // ============================================================

    private async void Probar_Click(object sender, RoutedEventArgs e)
    {
        BotonProbar.IsEnabled = false;
        ResultadoPrueba.Text = "Conectando…";
        ResultadoPrueba.Foreground = Apariencia.Pincel("Ink3Brush");

        try
        {
            var motivo = await _repositorio.ProbarAsync(Componer());

            if (motivo.Length == 0)
            {
                ResultadoPrueba.Text = "Conecta.";
                ResultadoPrueba.Foreground = Apariencia.Pincel("OkBrush");
            }
            else
            {
                ResultadoPrueba.Text = motivo;
                ResultadoPrueba.Foreground = Apariencia.Pincel("BadBrush");
            }
        }
        catch (Exception ex)
        {
            ResultadoPrueba.Text = ex.Message;
            ResultadoPrueba.Foreground = Apariencia.Pincel("BadBrush");
        }
        finally
        {
            BotonProbar.IsEnabled = true;
        }
    }

    private void Guardar_Click(object sender, RoutedEventArgs e)
    {
        // No se exige que la conexión funcione para guardar: una empresa con VPN no conecta
        // hasta que se levanta el túnel, y registrarla antes es legítimo.
        Resultado = Componer();
        DialogResult = true;
        Close();
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
