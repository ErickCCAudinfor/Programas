using Convertidor.Conversion.Paginas;
using ImageMagick;

namespace Convertidor.Conversion;

/// <summary>
/// PDF entero a imagen, a 200 ppp, desde la ventana principal. Para sacar solo una zona, o
/// elegir la resolución y el tamaño, está la ventana de recorte.
///
///   - Una página: un archivo, "20.png".
///   - Varias: una carpeta "20 (PNG)" con "Página 01.png", "Página 02.png"...
///   - TIFF: un único .tiff con todas las páginas, que es como se suele archivar.
///
/// El PDF a Word lo sigue haciendo el conversor de Word: se juntan los dos en el catálogo.
/// </summary>
public sealed class ConversorPdf : IConversor
{
    public const double Ppp = 200;

    private static readonly FormatoDestino[] Salidas =
    {
        new("png",  "PNG",  ".png"),
        new("jpg",  "JPG",  ".jpg"),
        new("webp", "WEBP", ".webp"),
        new("tiff", "TIFF", ".tiff"),
        new("bmp",  "BMP",  ".bmp"),
    };

    public string Familia => "PDF";

    public bool Admite(string extension) => extension == ".pdf";

    public IReadOnlyList<FormatoDestino> Destinos(string ruta) => Salidas;

    public string Convertir(string ruta, FormatoDestino destino, string carpetaSalida, ContextoConversion contexto)
    {
        // Se llama desde el hilo de conversión, que no tiene contexto de sincronización:
        // esperar aquí a lo asíncrono de Windows no bloquea nada.
        using var pdf = FuentePdf.Abrir(ruta).GetAwaiter().GetResult();
        var formato = ConversorImagen.Formato(destino.Clave);
        var nombre = RutaSalida.NombreBase(ruta);

        MagickImage Pagina(int i)
        {
            var (w, h) = pdf.Tamano(i);
            var png = pdf.DibujarPng(i, Region.Entera, Fuentes.Px(w, Ppp), Fuentes.Px(h, Ppp), transparente: false)
                         .GetAwaiter().GetResult();
            var img = new MagickImage(png) { Density = new Density(Ppp, Ppp, DensityUnit.PixelsPerInch) };
            return img;
        }

        if (formato == MagickFormat.Tiff && pdf.Paginas > 1)
        {
            var salida = RutaSalida.Archivo(carpetaSalida, nombre, destino.Extension);
            using var todas = new MagickImageCollection();
            for (var i = 0; i < pdf.Paginas; i++) todas.Add(Pagina(i));
            todas.Write(salida, formato);
            return salida;
        }

        if (pdf.Paginas == 1)
        {
            var salida = RutaSalida.Archivo(carpetaSalida, nombre, destino.Extension);
            using var img = Pagina(0);
            ConversorImagen.Guardar(img, formato, salida);
            return salida;
        }

        var carpeta = RutaSalida.Carpeta(carpetaSalida, $"{nombre} ({destino.Nombre})");
        var cifras = Math.Max(2, pdf.Paginas.ToString().Length);
        for (var i = 0; i < pdf.Paginas; i++)
        {
            using var img = Pagina(i);
            ConversorImagen.Guardar(img, formato,
                Path.Combine(carpeta, $"Página {(i + 1).ToString().PadLeft(cifras, '0')}{destino.Extension}"));
        }
        return carpeta;
    }
}
