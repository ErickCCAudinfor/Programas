using System;
using System.Windows;
using System.Windows.Media;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Operaciones;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Traducciones de dominio a apariencia, en un solo sitio.
///
/// El Core no sabe de pinceles ni de iconos a propósito, y varias vistas necesitan el mismo
/// mapeo (el chip de la barra superior, las tarjetas de acceso, las filas de la tabla). Antes
/// de que se duplicara por tercera vez, aquí está.
/// </summary>
public static class Apariencia
{
    public static Brush Pincel(string clave) =>
        Application.Current?.TryFindResource(clave) as Brush ?? Brushes.Transparent;

    public static Geometry Icono(string clave) =>
        Application.Current?.TryFindResource(clave) as Geometry ?? Geometry.Empty;

    // ---------- Entorno ----------

    /// <summary>Color vivo: puntos, líneas de entorno y rellenos pequeños.</summary>
    public static Brush MarcaEntorno(ClaveEntorno? entorno) => Pincel(entorno switch
    {
        ClaveEntorno.Produccion => "ProMarkBrush",
        ClaveEntorno.Replica => "RepMarkBrush",
        ClaveEntorno.Uat => "UatMarkBrush",
        _ => "HairBrush"
    });

    /// <summary>Color oscurecido: el que se usa cuando hace de texto.</summary>
    public static Brush TextoEntorno(ClaveEntorno? entorno) => Pincel(entorno switch
    {
        ClaveEntorno.Produccion => "ProBrush",
        ClaveEntorno.Replica => "RepBrush",
        ClaveEntorno.Uat => "UatBrush",
        _ => "Ink3Brush"
    });

    public static Brush FondoEntorno(ClaveEntorno? entorno) => Pincel(entorno switch
    {
        ClaveEntorno.Produccion => "ProSoftBrush",
        ClaveEntorno.Replica => "RepSoftBrush",
        ClaveEntorno.Uat => "UatSoftBrush",
        _ => "SunkBrush"
    });

    public static Brush BordeEntorno(ClaveEntorno? entorno) => Pincel(entorno switch
    {
        ClaveEntorno.Produccion => "ProLineBrush",
        ClaveEntorno.Replica => "RepLineBrush",
        ClaveEntorno.Uat => "UatLineBrush",
        _ => "HairBrush"
    });

    public static string AbreviaturaEntorno(ClaveEntorno? entorno) => entorno switch
    {
        ClaveEntorno.Produccion => "PRO",
        ClaveEntorno.Replica => "REP",
        ClaveEntorno.Uat => "UAT",
        _ => "—"
    };

    // ---------- Secciones ----------

    /// <summary>
    /// Icono de una sección a partir de su nombre. El Core devuelve el grupo como texto
    /// («Precios», «Consultas»…) y aquí se elige con qué se dibuja.
    /// </summary>
    public static string ClaveIconoDeSeccion(SeccionOperacion seccion) => seccion switch
    {
        SeccionOperacion.Precios => "IcoPrecios",
        SeccionOperacion.Contratos => "IcoContratos",
        SeccionOperacion.Productos => "IcoProductos",
        SeccionOperacion.Consultas => "IcoConsultas",
        SeccionOperacion.Facturas => "IcoFacturas",
        _ => "IcoAjustes"
    };

    public static string ClaveIconoDeGrupo(string? grupo) => grupo switch
    {
        "Precios" => "IcoPrecios",
        "Contratos" => "IcoContratos",
        "Productos" => "IcoProductos",
        "Consultas" => "IcoConsultas",
        "Facturas y PDF" => "IcoFacturas",
        "Ajustes" => "IcoAjustes",
        _ => "IcoEjecuciones"
    };

    // ---------- Tiempo ----------

    /// <summary>
    /// «hace 20 min», «hace 3 h», «ayer», «hace 3 días». Se queda en lo aproximado a
    /// propósito: en el inicio interesa el orden de magnitud, no la hora exacta.
    /// </summary>
    public static string TiempoRelativo(DateTime momento, DateTime? ahora = null)
    {
        var referencia = ahora ?? DateTime.Now;
        var transcurrido = referencia - momento;

        if (transcurrido < TimeSpan.Zero) return "ahora";
        if (transcurrido < TimeSpan.FromMinutes(1)) return "hace un momento";
        if (transcurrido < TimeSpan.FromHours(1)) return $"hace {(int)transcurrido.TotalMinutes} min";
        if (transcurrido < TimeSpan.FromHours(24)) return $"hace {(int)transcurrido.TotalHours} h";

        var dias = (int)transcurrido.TotalDays;
        return dias switch
        {
            1 => "ayer",
            < 7 => $"hace {dias} días",
            _ => momento.ToString("dd/MM")
        };
    }

    /// <summary>
    /// Duración corta para la interfaz: «2:14», «0:52», «0,4 s».
    ///
    /// Por debajo del segundo se muestran décimas en lugar de «0:00», que no dice nada y hace
    /// pensar que la operación no llegó a ejecutarse.
    /// </summary>
    public static string Duracion(TimeSpan duracion)
    {
        if (duracion < TimeSpan.FromSeconds(1))
        {
            return $"{duracion.TotalSeconds:0.0} s";
        }
        return $"{(int)duracion.TotalMinutes}:{duracion.Seconds:00}";
    }
}
