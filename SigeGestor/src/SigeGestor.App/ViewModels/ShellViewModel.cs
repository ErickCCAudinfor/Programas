using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Operaciones;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Estado del shell: quién ha entrado, contra qué entorno se está mirando y en qué sección
/// estamos. No conoce el contenido de cada sección, solo cuál está activa.
/// </summary>
public sealed partial class ShellViewModel : ObservableObject
{
    public ShellViewModel(Usuario usuario, EntornoBD? entorno)
    {
        Usuario = usuario;
        Entorno = entorno;

        // Los contadores salen del catálogo, no a mano: si se añade una operación, la
        // barra lateral se actualiza sola.
        Navegacion = new ObservableCollection<ItemNavegacion>
        {
            new() { Seccion = Seccion.Inicio,      Titulo = "Inicio",      ClaveIcono = "IcoInicio" },
            new() { Seccion = Seccion.Ejecuciones, Titulo = "Ejecuciones", ClaveIcono = "IcoEjecuciones" },

            DeCatalogo(Seccion.Precios,   SeccionOperacion.Precios,   "Precios",        "IcoPrecios"),
            DeCatalogo(Seccion.Contratos, SeccionOperacion.Contratos, "Contratos",      "IcoContratos"),
            DeCatalogo(Seccion.Productos, SeccionOperacion.Productos, "Productos",      "IcoProductos"),
            DeCatalogo(Seccion.Consultas, SeccionOperacion.Consultas, "Consultas",      "IcoConsultas"),
            DeCatalogo(Seccion.Facturas,  SeccionOperacion.Facturas,  "Facturas y PDF", "IcoFacturas"),

            DeCatalogo(Seccion.Ajustes, SeccionOperacion.Ajustes, "Ajustes", "IcoAjustes", "Sistema"),
            new() { Seccion = Seccion.Novedades, Titulo = "Novedades", Grupo = "Sistema", ClaveIcono = "IcoNovedades" }
        };

        RefrescarAvisos();

        VistaNavegacion = new CollectionViewSource { Source = Navegacion };
        VistaNavegacion.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ItemNavegacion.Grupo)));

        _seccionActual = Navegacion[0];
    }

    /// <summary>
    /// Recalcula el número de novedades sin leer de esta persona.
    ///
    /// Antes estaba puesto a mano —«Avisos = 3»—, heredado de la maqueta: el número no cambiaba
    /// nunca, ni al abrir las novedades ni al publicar una versión nueva.
    /// </summary>
    public void RefrescarAvisos()
    {
        var novedades = Navegacion.FirstOrDefault(i => i.Seccion == Seccion.Novedades);
        if (novedades is null) return;

        novedades.Avisos = NovedadesViewModel.SinLeer(Usuario);

        OnPropertyChanged(nameof(Avisos));
        OnPropertyChanged(nameof(VisibilidadAvisos));
        OnPropertyChanged(nameof(AyudaAvisos));
    }

    /// <summary>Novedades sin leer. Lo comparten la campana y la barra lateral.</summary>
    public int Avisos =>
        Navegacion.FirstOrDefault(i => i.Seccion == Seccion.Novedades)?.Avisos ?? 0;

    public Visibility VisibilidadAvisos =>
        Avisos > 0 ? Visibility.Visible : Visibility.Collapsed;

    public string AyudaAvisos => Avisos switch
    {
        0 => "Novedades · estás al día",
        1 => "Novedades · 1 versión sin leer",
        _ => $"Novedades · {Avisos} versiones sin leer"
    };

    /// <summary>Lleva a una sección desde fuera de la barra lateral, como hace la campana.</summary>
    public void Ir(Seccion seccion)
    {
        var destino = Navegacion.FirstOrDefault(i => i.Seccion == seccion);
        if (destino is not null) SeccionActual = destino;
    }


    private static ItemNavegacion DeCatalogo(Seccion seccion,
                                             SeccionOperacion seccionCatalogo,
                                             string titulo,
                                             string claveIcono,
                                             string grupo = "Operaciones")
    {
        return new ItemNavegacion
        {
            Seccion = seccion,
            SeccionCatalogo = seccionCatalogo,
            Titulo = titulo,
            Grupo = grupo,
            ClaveIcono = claveIcono,
            Contador = CatalogoOperaciones.CuantasEn(seccionCatalogo).ToString(),
        };
    }

    public Usuario Usuario { get; }

    public EntornoBD? Entorno { get; }

    public ObservableCollection<ItemNavegacion> Navegacion { get; }

    /// <summary>Agrupada por <see cref="ItemNavegacion.Grupo"/> para los encabezados.</summary>
    public CollectionViewSource VistaNavegacion { get; }

    [ObservableProperty]
    private ItemNavegacion _seccionActual;

    partial void OnSeccionActualChanged(ItemNavegacion value)
    {
        OnPropertyChanged(nameof(TituloSeccion));
    }

    // ---------- Identidad ----------

    public string Iniciales => Usuario.Iniciales;

    public string NombreUsuario => Usuario.Nombre;

    public string RolUsuario => Usuario.EsAccesoInformes ? "Acceso de informes" : "Operador";

    // ---------- Sección ----------

    public string TituloSeccion => SeccionActual?.Titulo ?? "";

    // ---------- Entorno ----------

    public string NombreEntorno => Entorno?.Nombre ?? "Sin entorno";

    public string ServidorEntorno => Entorno?.Servidor ?? "";

    public string DetalleEntorno => Entorno is null
        ? "No hay ningún entorno configurado."
        : $"{Entorno.Abreviatura} · {Entorno.Descripcion}";

    /// <summary>
    /// El color de la hairline de 2px bajo la barra superior. Es la firma del sistema de
    /// diseño: siempre a la vista y sin pedir atención.
    /// </summary>
    public Brush ColorEntorno => Apariencia.MarcaEntorno(Entorno?.Clave);

    public Brush FondoEntorno => Apariencia.FondoEntorno(Entorno?.Clave);

    public Brush BordeEntorno => Apariencia.BordeEntorno(Entorno?.Clave);

    public Brush TextoEntorno => Apariencia.TextoEntorno(Entorno?.Clave);
}
