namespace Convertidor.Conversion.Paginas;

/// <summary>
/// Zona de una página en proporciones de 0 a 1, para que no dependa del zoom ni de la
/// resolución: (0, 0, 1, 1) es la página entera.
/// </summary>
public sealed record Region(double X, double Y, double Ancho, double Alto)
{
    public static readonly Region Entera = new(0, 0, 1, 1);
}

/// <summary>
/// Algo con páginas que se pueden dibujar: un PDF, o una imagen (un TIFF puede traer varias).
///
/// Las medidas van en DIP (1/96 de pulgada), la unidad de WPF. Así, la vista al 100 % mide lo
/// mismo que el original, y los píxeles de salida salen de una sola cuenta:
///     píxeles = DIP × ppp / 96
/// </summary>
public interface IFuentePaginas : IDisposable
{
    string Ruta { get; }
    int Paginas { get; }

    /// <summary>Tamaño de la página en DIP. La página va de 0 a Paginas-1.</summary>
    (double Ancho, double Alto) Tamano(int pagina);

    /// <summary>La resolución que se propone al abrir: 200 en un PDF, la suya en una imagen.</summary>
    double PppPropuesta { get; }

    /// <summary>
    /// Si el fondo puede ser transparente. En un PDF sí (las páginas no tienen fondo propio);
    /// en una imagen, la transparencia es la que ya traiga.
    /// </summary>
    bool AdmiteFondoTransparente { get; }

    /// <summary>Dibuja la zona de la página al tamaño en píxeles pedido y la devuelve como PNG.</summary>
    Task<byte[]> DibujarPng(int pagina, Region region, int pxAncho, int pxAlto, bool transparente);
}

public static class Fuentes
{
    public static bool EsRecortable(string ruta)
    {
        var ext = Path.GetExtension(ruta).ToLowerInvariant();
        return ext == ".pdf" || new ConversorImagen().Admite(ext);
    }

    public static async Task<IFuentePaginas> Abrir(string ruta) =>
        Path.GetExtension(ruta).Equals(".pdf", StringComparison.OrdinalIgnoreCase)
            ? await FuentePdf.Abrir(ruta)
            : await Task.Run(() => (IFuentePaginas)new FuenteImagen(ruta));

    /// <summary>Píxeles que salen de una medida en DIP a una resolución.</summary>
    public static int Px(double dip, double ppp) => Math.Max(1, (int)Math.Round(dip * ppp / 96.0));
}
