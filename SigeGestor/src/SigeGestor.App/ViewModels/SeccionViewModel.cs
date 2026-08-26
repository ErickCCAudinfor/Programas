using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Operaciones;

namespace SigeGestor.App.ViewModels;

/// <summary>Una operación tal como se lista en la página de su sección.</summary>
public sealed class OperacionItemVm
{
    public OperacionItemVm(DefinicionOperacion definicion)
    {
        Definicion = definicion;
        Subgrupo = definicion.Subgrupo;
    }

    public DefinicionOperacion Definicion { get; }

    public string Subgrupo { get; }

    public string Nombre => Definicion.Nombre;

    public string Descripcion => Definicion.Descripcion;

    /// <summary>Icono de la sección: todas las operaciones de una sección comparten el suyo.</summary>
    public Geometry Icono => Apariencia.Icono(Apariencia.ClaveIconoDeSeccion(Definicion.Seccion));

    public ClaveEntorno Entorno => Definicion.EntornoPorDefecto;

    public string AbreviaturaEntorno => Apariencia.AbreviaturaEntorno(Entorno);

    public Brush MarcaEntorno => Apariencia.MarcaEntorno(Entorno);

    public Brush TextoEntorno => Apariencia.TextoEntorno(Entorno);

    public Brush FondoEntorno => Apariencia.FondoEntorno(Entorno);

    public Brush BordeEntorno => Apariencia.BordeEntorno(Entorno);

    /// <summary>Las de escritura se marcan: es la información que evita disgustos.</summary>
    public Visibility VisibilidadEscritura =>
        Definicion.EsEscritura ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Mientras no esté portada se dice, en vez de dejar un botón muerto.</summary>
    public Visibility VisibilidadPendiente =>
        Definicion.Implementada ? Visibility.Collapsed : Visibility.Visible;

    /// <summary>Qué entradas pide, en lenguaje llano: «Contratos · Fechas · Carpeta».</summary>
    public string Entradas
    {
        get
        {
            var partes = new List<string>();
            if (Definicion.Pide(EntradasOperacion.Contratos)) partes.Add("Contratos");
            if (Definicion.Pide(EntradasOperacion.Cups)) partes.Add("CUPS");
            if (Definicion.Pide(EntradasOperacion.Cifs)) partes.Add("CIF");
            if (Definicion.Pide(EntradasOperacion.Facturas)) partes.Add("Facturas");
            if (Definicion.Pide(EntradasOperacion.GrupoTarifa)) partes.Add("Grupo de tarifa");
            if (Definicion.Pide(EntradasOperacion.FiltroTarifaActual)) partes.Add("Grupo actual");
            if (Definicion.Pide(EntradasOperacion.Fechas)) partes.Add("Fechas");
            if (Definicion.Pide(EntradasOperacion.Texto)) partes.Add(
                string.IsNullOrEmpty(Definicion.EtiquetaTexto) ? "Texto" : Definicion.EtiquetaTexto);
            if (Definicion.Pide(EntradasOperacion.Excel)) partes.Add("Excel");
            if (Definicion.Pide(EntradasOperacion.Carpeta)) partes.Add("Carpeta");

            return partes.Count == 0 ? "No necesita datos" : string.Join(" · ", partes);
        }
    }
}

/// <summary>
/// Página de una sección: la lista de sus operaciones, agrupadas por subgrupo cuando lo hay.
///
/// Toda la página sale del catálogo. Añadir una operación no obliga a tocar esta vista.
/// </summary>
public sealed class SeccionViewModel
{
    public SeccionViewModel(SeccionOperacion seccion, string titulo, string descripcion, string claveIcono)
    {
        Seccion = seccion;
        Titulo = titulo;
        Descripcion = descripcion;
        ClaveIcono = claveIcono;

        var operaciones = CatalogoOperaciones.DeSeccion(seccion)
                                             .Select(d => new OperacionItemVm(d))
                                             .ToList();

        Operaciones = operaciones;

        Vista = new CollectionViewSource { Source = operaciones };
        Vista.GroupDescriptions.Add(new PropertyGroupDescription(nameof(OperacionItemVm.Subgrupo)));
    }

    public SeccionOperacion Seccion { get; }

    public string Titulo { get; }

    public string Descripcion { get; }

    public string ClaveIcono { get; }

    public Geometry Icono => Apariencia.Icono(ClaveIcono);

    public IReadOnlyList<OperacionItemVm> Operaciones { get; }

    public CollectionViewSource Vista { get; }

    public string Recuento => Operaciones.Count == 1
        ? "1 operación"
        : $"{Operaciones.Count} operaciones";

    /// <summary>Cuántas están portadas ya. Se muestra mientras queden pendientes.</summary>
    public string Progreso
    {
        get
        {
            var hechas = Operaciones.Count(o => o.Definicion.Implementada);
            return hechas == Operaciones.Count
                ? "todas disponibles"
                : $"{hechas} de {Operaciones.Count} disponibles";
        }
    }
}
