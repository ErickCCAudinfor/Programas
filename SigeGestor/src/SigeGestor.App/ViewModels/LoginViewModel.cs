using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Servicios;
using SigeGestor.Core.Sesion;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Lógica del inicio de sesión.
///
/// La contraseña no vive aquí: PasswordBox no la expone como propiedad enlazable, así que
/// la vista la pasa al ejecutar el comando. Es la forma recomendada y evita tener la clave
/// en una propiedad observable de la que luego no controlas el ciclo de vida.
/// </summary>
public sealed partial class LoginViewModel : ObservableObject
{
    private readonly ServicioAutenticacion _autenticacion;
    private CancellationTokenSource? _cts;

    public LoginViewModel(ServicioAutenticacion autenticacion)
    {
        _autenticacion = autenticacion;
    }

    /// <summary>Se dispara cuando la autenticación ha ido bien. La vista cierra y sigue.</summary>
    public event EventHandler<Usuario>? AccesoConcedido;

    [ObservableProperty]
    private string _login = string.Empty;

    [ObservableProperty]
    private string _mensajeError = string.Empty;

    [ObservableProperty]
    private bool _validando;

    /// <summary>True cuando el error es de configuración: pide otra acción del usuario.</summary>
    [ObservableProperty]
    private bool _errorDeConfiguracion;

    public bool HayError => !string.IsNullOrEmpty(MensajeError);

    public string Version => "versión 3.0";

    public string TextoBoton => Validando ? "Validando…" : "Entrar";

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HayError));

    partial void OnValidandoChanged(bool value) => OnPropertyChanged(nameof(TextoBoton));

    partial void OnLoginChanged(string value) => LimpiarError();

    public void LimpiarError()
    {
        if (HayError)
        {
            MensajeError = string.Empty;
            ErrorDeConfiguracion = false;
        }
    }

    public async Task EntrarAsync(string clave)
    {
        if (Validando) return;

        if (string.IsNullOrWhiteSpace(Login) && string.IsNullOrWhiteSpace(clave))
        {
            MensajeError = "Escribe tu usuario y tu clave.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Login))
        {
            MensajeError = "Falta el usuario.";
            return;
        }

        if (string.IsNullOrWhiteSpace(clave))
        {
            MensajeError = "Falta la clave.";
            return;
        }

        Validando = true;
        MensajeError = string.Empty;
        ErrorDeConfiguracion = false;

        _cts?.Cancel();
        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            var respuesta = await _autenticacion.AutenticarAsync(Login, clave, _cts.Token);

            if (respuesta.Correcto)
            {
                SesionActual.Iniciar(respuesta.Usuario);
                AccesoConcedido?.Invoke(this, respuesta.Usuario);
                return;
            }

            ErrorDeConfiguracion = respuesta.Resultado == ResultadoLogin.ConfiguracionIncompleta;
            MensajeError = respuesta.Mensaje;
        }
        catch (Exception ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            Validando = false;
        }
    }
}
