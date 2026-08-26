using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SigeGestor.App.ViewModels;

public enum Tendencia
{
    Plana,
    Sube,
    Baja
}

/// <summary>
/// Una tarjeta de métrica del inicio.
///
/// Dirección y bondad van separadas a propósito: la flecha dice si la cifra ha subido o
/// bajado, y el color dice si eso es bueno. En «Con error», bajar es bueno; en «Ejecuciones»,
/// subir es normal. Meterlo todo en un solo color mentiría en una de las dos.
/// </summary>
public sealed class MetricaVm
{
    public required string Titulo { get; init; }

    public required string ClaveIcono { get; init; }

    /// <summary>Cifra ya formateada. El formato es cosa de la vista, no del Core.</summary>
    public required string Valor { get; init; }

    /// <summary>Sufijo pequeño junto a la cifra: «M», «€». Null para no mostrarlo.</summary>
    public string? Unidad { get; init; }

    /// <summary>Texto del delta: «6», «12 %», «estable». Null oculta el bloque.</summary>
    public string? Delta { get; init; }

    public Tendencia Tendencia { get; init; }

    /// <summary>True cuando lo deseable es que la cifra baje (errores, duración).</summary>
    public bool MejorEsMenos { get; init; }

    /// <summary>Pinta la cifra en rojo. Para «Con error» cuando hay alguno.</summary>
    public bool Alarmante { get; init; }

    public IReadOnlyList<double> Serie { get; init; } = Array.Empty<double>();

    // ---------- Derivados para la vista ----------

    public Geometry Icono => Apariencia.Icono(ClaveIcono);

    public Visibility VisibilidadDelta =>
        string.IsNullOrEmpty(Delta) ? Visibility.Collapsed : Visibility.Visible;

    public Visibility VisibilidadFlecha =>
        Tendencia == Tendencia.Plana ? Visibility.Collapsed : Visibility.Visible;

    public Geometry FlechaIcono => Tendencia switch
    {
        Tendencia.Sube => Apariencia.Icono("IcoFlechaArriba"),
        Tendencia.Baja => Apariencia.Icono("IcoFlechaAbajo"),
        _ => Geometry.Empty
    };

    /// <summary>Verde si el movimiento es el deseable, rojo si no, neutro si no se mueve.</summary>
    public Brush ColorDelta
    {
        get
        {
            if (Tendencia == Tendencia.Plana) return Apariencia.Pincel("Ink4Brush");

            var haMejorado = MejorEsMenos
                ? Tendencia == Tendencia.Baja
                : Tendencia == Tendencia.Sube;

            return Apariencia.Pincel(haMejorado ? "OkBrush" : "BadBrush");
        }
    }

    public Brush ColorValor => Apariencia.Pincel(Alarmante ? "BadBrush" : "InkBrush");

    public Brush TrazoSparkline => Apariencia.Pincel(Alarmante ? "BadBrush" : "Ink3Brush");

    public Brush RellenoSparkline => Apariencia.Pincel(Alarmante ? "BadSoftBrush" : "Hair2Brush");

    public Brush PuntoSparkline => Apariencia.Pincel(Alarmante ? "BadBrush" : "InkBrush");
}
