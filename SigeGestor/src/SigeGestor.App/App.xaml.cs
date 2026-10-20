using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;
using SigeGestor.App.ViewModels;
using SigeGestor.App.Views;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core.Registro;
using SigeGestor.Core.Servicios;

namespace SigeGestor.App;

public partial class App : Application
{
    /// <summary>
    /// Días de log que se conservan. Erick quiere borrarlo cada 2-3 días, así que las series
    /// de los sparkline son tramos de 3 h sobre las últimas 24 h: ocho días de historia no
    /// caben en esta ventana.
    /// </summary>
    private const int DiasRetencionLog = 3;

    private RepositorioEntornos _entornos = null!;
    private ServicioAutenticacion _autenticacion = null!;
    private RegistroEjecuciones _registro = null!;
    private IRepositorioEjecuciones _ejecuciones = null!;

    // ============================================================
    // QUIÉN ESTÁ DENTRO
    //
    // Se apunta en Config\Usuarios.json, que al vivir el .exe en el .13 es el mismo fichero
    // para las ocho personas. Sirve para saber a quién avisar antes de actualizar la
    // aplicación: la copia va con robocopy sobre el .exe y, si alguien lo tiene abierto, el
    // fichero está bloqueado y la publicación falla.
    //
    // El latido es lo que distingue «está dentro» de «se quedó marcado». Ver EstadoUsuario.
    // ============================================================

    private readonly RepositorioEstadoUsuario _estadoUsuario = new();
    private DispatcherTimer? _latido;
    private string _loginEnCurso = "";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        FijarCulturaEspanola();

        // El cierre se controla a mano de principio a fin. No se puede usar
        // OnMainWindowClose porque al cerrar la ventana principal queremos volver al
        // login, no salir: WPF apagaría la aplicación antes de poder reabrirlo.
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        _entornos = new RepositorioEntornos();
        _autenticacion = new ServicioAutenticacion(_entornos);

        // El histórico son ficheros de log junto al ejecutable. Como el .exe vive en el
        // servidor .13 y todo el equipo lo abre desde ahí, el log queda compartido sin
        // más: es el mismo mecanismo del Usuarios.json de ActualizaPrecios.
        _registro = new RegistroEjecuciones { DiasRetencion = DiasRetencionLog };
        _ejecuciones = new RepositorioEjecucionesLog(_registro);

        // Se barre lo que ya está fuera de la ventana. Es síncrono pero solo lista y borra
        // unos pocos ficheros pequeños.
        _registro.Purgar();

        PedirAcceso();
    }

    /// <summary>
    /// WPF formatea las fechas y los números de los enlaces en en-US por defecto, sin hacer
    /// caso a la cultura del hilo: la propiedad Language de FrameworkElement viene fijada a
    /// "en-US". Hay que cambiar las dos cosas o «1,86» se vería como «1.86».
    /// </summary>
    private static void FijarCulturaEspanola()
    {
        var cultura = new CultureInfo("es-ES");

        CultureInfo.DefaultThreadCurrentCulture = cultura;
        CultureInfo.DefaultThreadCurrentUICulture = cultura;
        CultureInfo.CurrentCulture = cultura;
        CultureInfo.CurrentUICulture = cultura;

        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(cultura.IetfLanguageTag)));
    }

    /// <summary>
    /// Muestra el login y, si se concede el acceso, abre la ventana principal. Al cerrarse
    /// esta se vuelve aquí, de forma que cambiar de usuario no obliga a relanzar el .exe.
    /// La aplicación solo termina cuando se cancela el login.
    /// </summary>
    private void PedirAcceso()
    {
        var login = new LoginWindow(new LoginViewModel(_autenticacion));

        if (login.ShowDialog() != true || login.UsuarioAutenticado is null)
        {
            Shutdown();
            return;
        }

        var usuario = login.UsuarioAutenticado;

        AbrirSesion(usuario.Login, usuario.Nombre);

        var principal = new ShellWindow(
            usuario, _entornos, _ejecuciones, _registro, DiasRetencionLog);
        MainWindow = principal;

        // Se encola en el Dispatcher en vez de llamar directamente: así la ventana que se
        // está cerrando termina de destruirse antes de abrir el login, y la llamada no se
        // apila sobre la anterior.
        principal.Closed += (_, _) =>
        {
            CerrarSesion();
            Dispatcher.BeginInvoke(new Action(PedirAcceso));
        };

        principal.Show();
    }

    /// <summary>
    /// Apunta la entrada y arranca el latido. Se vuelve a pasar por aquí al cambiar de
    /// usuario sin cerrar el .exe, así que primero se cierra lo que hubiera abierto.
    /// </summary>
    private void AbrirSesion(string login, string nombre)
    {
        CerrarSesion();

        _loginEnCurso = login;
        _estadoUsuario.MarcarEntrada(login, nombre);

        _latido = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(RepositorioEstadoUsuario.MinutosEntreLatidos)
        };
        _latido.Tick += (_, _) => _estadoUsuario.Latido(_loginEnCurso);
        _latido.Start();
    }

    private void CerrarSesion()
    {
        _latido?.Stop();
        _latido = null;

        if (_loginEnCurso.Length == 0) return;

        _estadoUsuario.MarcarSalida(_loginEnCurso);
        _loginEnCurso = "";
    }

    /// <summary>
    /// Por si se sale por una vía que no pasa por cerrar la ventana —cancelar el login con el
    /// shell ya cerrado, o un Shutdown—. Lo que NO se puede cubrir es matar el proceso o
    /// apagar la máquina: para eso está el latido.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        CerrarSesion();
        base.OnExit(e);
    }
}
