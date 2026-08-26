using System.Windows.Media;
using SigeGestor.Core.Modelos;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Tarjeta de acceso rápido. Son por usuario, no fijas: es lo que permite que ocho perfiles
/// distintos entren cada uno directo a lo suyo sin una navegación que sirva a todos a medias.
/// </summary>
public sealed class AccesoVm
{
    public required string Titulo { get; init; }

    public required string Grupo { get; init; }

    public required ClaveEntorno Entorno { get; init; }

    /// <summary>«hace 3 h · 216 contratos»</summary>
    public required string Detalle { get; init; }

    /// <summary>La primera tarjeta se destaca: es la que más usa esa persona.</summary>
    public bool Destacada { get; init; }

    public Geometry Icono => Apariencia.Icono(Apariencia.ClaveIconoDeGrupo(Grupo));

    public string AbreviaturaEntorno => Apariencia.AbreviaturaEntorno(Entorno);

    public Brush MarcaEntorno => Apariencia.MarcaEntorno(Entorno);

    public Brush TextoEntorno => Apariencia.TextoEntorno(Entorno);

    public Brush BordeTarjeta => Apariencia.Pincel(Destacada ? "AccentLineBrush" : "HairBrush");
}
