using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core.Modelos;

namespace SigeGestor.App.ViewModels;

/// <summary>Una versión del historial, lista para pintar.</summary>
public sealed class NovedadVm
{
    public required Novedad Novedad { get; init; }

    public string Version => $"Versión {Novedad.Version}";
    public string Fecha => Novedad.Fecha.ToString("d 'de' MMMM 'de' yyyy");
    public string Titulo => Novedad.Titulo ?? "";
    public IReadOnlyList<string> Cambios => Novedad.Cambios ?? new List<string>();

    public string Recuento => Cambios.Count == 1 ? "1 cambio" : $"{Cambios.Count} cambios";

    /// <summary>True si esta versión es posterior a la que la persona ya había visto.</summary>
    public required bool EsNueva { get; init; }

    public Visibility VisibilidadNueva => EsNueva ? Visibility.Visible : Visibility.Collapsed;
}

/// <summary>
/// La pantalla de novedades: qué ha cambiado en la aplicación.
///
/// El historial es <see cref="NovedadesApp"/>, ya portado de ActualizaPrecios; lo que no
/// existía en SigeGestor era dónde guardar qué versión ha visto cada uno. Eso lo pone ahora
/// <see cref="RepositorioEstadoUsuario"/> en Config\Usuarios.json, igual que en la aplicación
/// anterior y compartido por el mismo mecanismo: el .exe vive en el .13.
///
/// ABRIR ESTA PANTALLA MARCA COMO LEÍDO. Es lo que hacía la campana del Form1, y es lo que
/// espera cualquiera: si lo has visto, deja de avisarte. Se guarda al abrir y no al cerrar,
/// porque cerrar la aplicación por el aspa no dispararía nada.
/// </summary>
public sealed partial class NovedadesViewModel : ObservableObject
{
    private readonly RepositorioEstadoUsuario _estado = new();
    private readonly Usuario _usuario;

    public NovedadesViewModel(Usuario usuario)
    {
        _usuario = usuario;

        var leida = _estado.Obtener(usuario.Login).VersionNovedadesLeida;

        var historial = NovedadesApp.Historial ?? new List<Novedad>();

        // La primera vez —sin versión leída— solo la más reciente se marca como nueva. Marcar
        // el historial entero no informa de nada.
        var primeraVez = string.IsNullOrWhiteSpace(leida);

        for (var i = 0; i < historial.Count; i++)
        {
            var n = historial[i];
            var esNueva = primeraVez
                ? i == 0
                : NovedadesApp.EsPosterior(n.Version, leida);

            var vm = new NovedadVm { Novedad = n, EsNueva = esNueva };

            if (i == 0) Actual = vm;
            else Anteriores.Add(vm);
        }

        NuevasAlAbrir = historial.Count(n => primeraVez
            ? n.Version == historial[0].Version
            : NovedadesApp.EsPosterior(n.Version, leida));
    }

    /// <summary>La versión más reciente. Va destacada arriba.</summary>
    public NovedadVm? Actual { get; }

    /// <summary>El resto del historial.</summary>
    public ObservableCollection<NovedadVm> Anteriores { get; } = new();

    public Visibility VisibilidadAnteriores =>
        Anteriores.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

    /// <summary>Cuántas había sin leer justo antes de abrir. Para decirlo al entrar.</summary>
    public int NuevasAlAbrir { get; }

    public string TextoNuevas => NuevasAlAbrir switch
    {
        0 => "Ya estabas al día.",
        1 => "Hay 1 versión que no habías visto.",
        _ => $"Hay {NuevasAlAbrir} versiones que no habías visto."
    };

    public string VersionInstalada => $"Tienes instalada la {NovedadesApp.VersionActual}.";

    /// <summary>
    /// Marca todo como leído. Lo llama la vista al mostrarse.
    ///
    /// El objeto Usuario en memoria se actualiza también: si no, la barra lateral seguiría
    /// enseñando el aviso hasta reiniciar.
    /// </summary>
    public void MarcarLeido()
    {
        var version = NovedadesApp.VersionActual;
        if (string.IsNullOrWhiteSpace(version)) return;
        if (_usuario.VersionNovedadesLeida == version) return;

        _usuario.VersionNovedadesLeida = version;

        _estado.Guardar(new EstadoUsuario
        {
            Login = _usuario.Login,
            VersionNovedadesLeida = version
        });

        Leido?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Avisa al shell de que puede apagar el número de la barra lateral.</summary>
    public event EventHandler? Leido;

    /// <summary>
    /// Cuántas novedades sin leer tiene esta persona. Lo usa el shell para el número de la
    /// barra lateral, sin construir la pantalla.
    /// </summary>
    public static int SinLeer(Usuario usuario)
    {
        if (usuario is null) return 0;

        // Se prefiere lo que ya haya en memoria: si acaba de marcarlas, no hace falta volver
        // al fichero del .13 por SMB.
        var leida = !string.IsNullOrWhiteSpace(usuario.VersionNovedadesLeida)
            ? usuario.VersionNovedadesLeida
            : new RepositorioEstadoUsuario().Obtener(usuario.Login).VersionNovedadesLeida;

        return NovedadesApp.SinLeer(leida);
    }
}
