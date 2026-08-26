using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;
using SigeGestor.Core.Operaciones;

namespace SigeGestor.App.ViewModels;

/// <summary>Las secciones de la aplicación. El orden es el de la barra lateral.</summary>
public enum Seccion
{
    Inicio,
    Ejecuciones,
    Precios,
    Contratos,
    Productos,
    Consultas,
    Facturas,
    Ajustes,
    Novedades
}

/// <summary>
/// Una entrada de la barra lateral. Se agrupan por <see cref="Grupo"/>; las que llevan grupo
/// vacío van arriba sin encabezado.
/// </summary>
public sealed partial class ItemNavegacion : ObservableObject
{
    public required Seccion Seccion { get; init; }

    public required string Titulo { get; init; }

    public string Grupo { get; init; } = "";

    /// <summary>Clave de la geometría en Theme/Icons.xaml.</summary>
    public required string ClaveIcono { get; init; }

    /// <summary>
    /// Sección del catálogo que esta entrada abre, si la abre. Null en Inicio, Ejecuciones
    /// y Novedades, que no listan operaciones.
    /// </summary>
    public SeccionOperacion? SeccionCatalogo { get; init; }

    /// <summary>Número de operaciones de la sección. Null para no mostrar nada.</summary>
    public string? Contador { get; init; }

    /// <summary>
    /// Avisos pendientes; si es mayor que cero se pinta la píldora roja.
    ///
    /// Es lo único mutable de esta clase, y por eso hereda de ObservableObject: al abrir las
    /// novedades hay que apagar el número sin reiniciar la aplicación. El resto de campos son
    /// de solo inicialización porque la barra lateral se construye una vez y no cambia.
    /// </summary>
    [ObservableProperty]
    private int _avisos;

    partial void OnAvisosChanged(int value)
    {
        OnPropertyChanged(nameof(VisibilidadAvisos));
        OnPropertyChanged(nameof(TextoAvisos));
    }

    public Geometry Icono =>
        Application.Current.TryFindResource(ClaveIcono) as Geometry ?? Geometry.Empty;

    public Visibility VisibilidadContador =>
        string.IsNullOrEmpty(Contador) ? Visibility.Collapsed : Visibility.Visible;

    public Visibility VisibilidadAvisos =>
        Avisos > 0 ? Visibility.Visible : Visibility.Collapsed;

    public string TextoAvisos => Avisos.ToString();
}
