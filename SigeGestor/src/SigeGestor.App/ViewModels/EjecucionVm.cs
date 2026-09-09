using System.Windows.Media;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SigeGestor.Core.Modelos;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Una fila de la actividad del equipo.
///
/// El estado se codifica en forma y en color, no solo en texto: con ocho personas tocando
/// Producción, lo que hace falta es que un fallo se vea sin leer la fila.
/// </summary>
public sealed partial class EjecucionVm : ObservableObject
{
    /// <summary>
    /// La ejecución viva, cuando esta fila es una que está corriendo AHORA en este equipo.
    /// Nothing en las del histórico, que ya no cambian.
    /// </summary>
    private readonly EjecucionViewModel? _vivo;

    /// <summary>
    /// Fila de una ejecución que está corriendo ahora mismo.
    ///
    /// HACE FALTA porque el histórico se escribe AL TERMINAR: mientras una operación corre no
    /// hay nada en el log, y por eso el filtro «En marcha» de la rejilla no encontraba nunca
    /// nada. Era una opción muerta.
    ///
    /// La fila se suscribe al progreso, así que el contador avanza en la rejilla sin rehacerla.
    /// </summary>
    public EjecucionVm(TareaEnCurso tarea, Action<EjecucionVm>? pedirCancelar = null)
    {
        _vivo = tarea.Ejecucion;
        _pedirCancelar = pedirCancelar;

        Titulo = tarea.Operacion;
        Subtitulo = tarea.Grupo;
        Usuario = "";
        Iniciales = "";
        Grupo = tarea.Grupo;
        Entorno = tarea.Entorno;
        Estado = EstadoEjecucion.EnCurso;
        Errores = 0;
        Registros = 0;
        Duracion = "—";
        Cuando = Apariencia.TiempoRelativo(tarea.Momento);

        _vivo.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(EjecucionViewModel.Terminada))
            {
                // Al terminar deja de poder cancelarse, y el botón tiene que desaparecer
                // aunque la rejilla todavía no se haya rehecho.
                OnPropertyChanged(nameof(VisibilidadCancelar));
                return;
            }

            if (e.PropertyName is not (nameof(EjecucionViewModel.Contador)
                                       or nameof(EjecucionViewModel.Procesados))) return;

            OnPropertyChanged(nameof(Resultado));
        };
    }

    // ============================================================
    // CANCELAR DESDE LA REJILLA
    // ============================================================
    //
    // El panel de la ejecución ya tenía su botón de Cancelar, pero solo se llegaba a él estando
    // dentro. Con una consulta larga —una de fechas de un mes entero— te vas a otra pantalla, la
    // ves corriendo en la rejilla y no podías pararla: había que volver a su formulario.
    //
    // Va por comando y no por Click porque la plantilla de la fila vive en un diccionario de
    // tema (Actividad.xaml), y un diccionario no tiene código detrás donde poner el manejador.

    private readonly Action<EjecucionVm>? _pedirCancelar;

    /// <summary>Solo las que corren de verdad en este equipo se pueden cancelar.</summary>
    public Visibility VisibilidadCancelar =>
        _vivo is not null && !_vivo.Terminada && _pedirCancelar is not null
            ? Visibility.Visible
            : Visibility.Collapsed;

    public string AyudaCancelar => $"Cancelar «{Titulo}». Lo ya procesado se queda como está.";

    /// <summary>
    /// Pide cancelar. NO cancela aquí: lo pasa a quien pinta la rejilla para que pregunte
    /// antes. Una consulta de cuarenta minutos no se tira por un clic de más.
    /// </summary>
    [RelayCommand]
    private void Cancelar() => _pedirCancelar?.Invoke(this);

    /// <summary>Cancela de verdad. Lo llama la página cuando el usuario ha confirmado.</summary>
    public void CancelarAhora() => _vivo?.Cancelar();

    public EjecucionVm(Ejecucion e)
    {
        Titulo = e.Operacion;
        Subtitulo = string.IsNullOrWhiteSpace(e.Detalle) ? e.Grupo : $"{e.Grupo} · {e.Detalle}";
        Usuario = e.NombreUsuario;
        Iniciales = IngenieriaIniciales(e.NombreUsuario);
        Grupo = e.Grupo;
        Entorno = e.Entorno;
        Estado = e.Estado;
        Errores = e.Errores;
        Registros = e.Registros;
        Duracion = e.Estado == EstadoEjecucion.EnCurso ? "—" : Apariencia.Duracion(e.Duracion);
        Cuando = Apariencia.TiempoRelativo(e.Momento);
    }

    public string Titulo { get; }
    public string Subtitulo { get; }
    public string Usuario { get; }
    public string Iniciales { get; }
    public string Grupo { get; }
    public ClaveEntorno Entorno { get; }
    public EstadoEjecucion Estado { get; }
    public int Errores { get; }
    public long Registros { get; }
    public string Duracion { get; }
    public string Cuando { get; }

    public Geometry Icono => Apariencia.Icono(Apariencia.ClaveIconoDeGrupo(Grupo));

    // ---------- Entorno ----------

    public string AbreviaturaEntorno => Apariencia.AbreviaturaEntorno(Entorno);
    public Brush MarcaEntorno => Apariencia.MarcaEntorno(Entorno);
    public Brush TextoEntorno => Apariencia.TextoEntorno(Entorno);
    public Brush FondoEntorno => Apariencia.FondoEntorno(Entorno);
    public Brush BordeEntorno => Apariencia.BordeEntorno(Entorno);

    // ---------- Resultado ----------

    public string Resultado => _vivo is not null ? _vivo.Contador : Estado switch
    {
        EstadoEjecucion.EnCurso => "en marcha",
        EstadoEjecucion.Cancelada => "—",
        EstadoEjecucion.SinCambios => "nada",
        _ when Errores > 0 => $"{Errores:N0} err",
        _ => Registros.ToString("N0")
    };

    public Brush ColorResultado => Apariencia.Pincel(Estado switch
    {
        EstadoEjecucion.ConErrores => "BadBrush",
        EstadoEjecucion.Cancelada => "Ink4Brush",
        EstadoEjecucion.SinCambios => "Ink4Brush",
        _ => "MarcaBrush"
    });

    // ---------- Estado ----------

    public string EstadoTexto => Estado switch
    {
        EstadoEjecucion.Completada => "Completada",
        EstadoEjecucion.ConErrores => "Con errores",
        EstadoEjecucion.EnCurso => "En curso",
        EstadoEjecucion.SinCambios => "Sin cambios",
        _ => "Cancelada"
    };

    public Brush EstadoFondo => Apariencia.Pincel(Estado switch
    {
        EstadoEjecucion.Completada => "OkSoftBrush",
        EstadoEjecucion.ConErrores => "BadSoftBrush",
        EstadoEjecucion.EnCurso => "AccentSoftBrush",
        _ => "SunkBrush"
    });

    public Brush EstadoBorde => Apariencia.Pincel(Estado switch
    {
        EstadoEjecucion.Completada => "OkLineBrush",
        EstadoEjecucion.ConErrores => "BadLineBrush",
        EstadoEjecucion.EnCurso => "AccentLineBrush",
        _ => "HairBrush"
    });

    public Brush EstadoColorTexto => Apariencia.Pincel(Estado switch
    {
        EstadoEjecucion.Completada => "OkBrush",
        EstadoEjecucion.ConErrores => "BadBrush",
        EstadoEjecucion.EnCurso => "AccentBrush",
        _ => "Ink3Brush"
    });

    private static string IngenieriaIniciales(string nombre)
    {
        var limpio = (nombre ?? "").Trim();
        if (limpio.Length == 0) return "?";

        var partes = limpio.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 1)
        {
            return partes[0][..System.Math.Min(2, partes[0].Length)].ToUpperInvariant();
        }
        return $"{partes[0][0]}{partes[^1][0]}".ToUpperInvariant();
    }
}
