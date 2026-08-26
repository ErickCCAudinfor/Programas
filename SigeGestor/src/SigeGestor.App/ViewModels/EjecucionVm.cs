using System.Windows.Media;
using SigeGestor.Core.Modelos;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Una fila de la actividad del equipo.
///
/// El estado se codifica en forma y en color, no solo en texto: con ocho personas tocando
/// Producción, lo que hace falta es que un fallo se vea sin leer la fila.
/// </summary>
public sealed class EjecucionVm
{
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

    public string Resultado => Estado switch
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
