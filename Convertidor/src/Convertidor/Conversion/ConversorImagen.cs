using ImageMagick;

namespace Convertidor.Conversion;

/// <summary>
/// Imágenes con Magick.NET, que va dentro del .exe y no necesita nada instalado.
///
/// El PDF no está entre las entradas: Magick solo sabe leerlo con Ghostscript instalado.
/// Como salida sí vale, para pasar una foto o un TIFF de varias páginas a PDF.
/// </summary>
public sealed class ConversorImagen : IConversor
{
    private static readonly HashSet<string> Entradas = new()
    {
        ".png", ".jpg", ".jpeg", ".jpe", ".jfif", ".bmp", ".gif", ".tif", ".tiff",
        ".ico", ".webp", ".heic", ".heif", ".avif", ".svg", ".psd", ".tga",
    };

    private static readonly FormatoDestino[] Salidas =
    {
        new("png",  "PNG",  ".png"),
        new("jpg",  "JPG",  ".jpg"),
        new("webp", "WEBP", ".webp"),
        new("bmp",  "BMP",  ".bmp"),
        new("gif",  "GIF",  ".gif"),
        new("tiff", "TIFF", ".tiff"),
        new("ico",  "Icono (.ico)", ".ico"),
        new("pdf",  "PDF",  ".pdf"),
    };

    public string Familia => "Imagen";

    public bool Admite(string extension) => Entradas.Contains(extension);

    public IReadOnlyList<FormatoDestino> Destinos(string ruta)
    {
        var ext = Path.GetExtension(ruta).ToLowerInvariant();
        // .jpeg y .tif son lo mismo que .jpg y .tiff: no tiene sentido ofrecérselos a sí mismos
        var propia = ext switch { ".jpeg" or ".jpe" or ".jfif" => ".jpg", ".tif" => ".tiff", _ => ext };
        return Salidas.Where(s => s.Extension != propia).ToArray();
    }

    public string Convertir(string ruta, FormatoDestino destino, string carpetaSalida, ContextoConversion contexto)
    {
        var salida = RutaSalida.Archivo(carpetaSalida, RutaSalida.NombreBase(ruta), destino.Extension);
        var formato = Formato(destino.Clave);

        using var todas = new MagickImageCollection(ruta);

        // GIF, TIFF y PDF admiten varias imágenes: un GIF animado sigue animado y un TIFF de
        // varias páginas da un PDF de varias páginas.
        if (formato is MagickFormat.Gif or MagickFormat.Tiff or MagickFormat.Pdf)
        {
            if (formato == MagickFormat.Gif) todas.Coalesce();
            todas.Write(salida, formato);
            return salida;
        }

        // El resto es una sola imagen. Se coge la más grande porque un .ico trae varios
        // tamaños y el primero suele ser el de 16x16.
        using var img = todas.OrderByDescending(i => (long)i.Width * i.Height).First().Clone();
        Guardar(img, formato, salida);
        return salida;
    }

    public static MagickFormat Formato(string clave) => clave switch
    {
        "png" => MagickFormat.Png,
        "jpg" => MagickFormat.Jpeg,
        "webp" => MagickFormat.WebP,
        "bmp" => MagickFormat.Bmp,
        "gif" => MagickFormat.Gif,
        "tiff" => MagickFormat.Tiff,
        "ico" => MagickFormat.Ico,
        "pdf" => MagickFormat.Pdf,
        _ => throw new NotSupportedException(clave),
    };

    /// <summary>
    /// Guarda una imagen suelta con los ajustes que pide cada formato. Lo usan también la
    /// conversión de PDF y la ventana de recorte, para que todo salga igual.
    /// </summary>
    public static void Guardar(IMagickImage<byte> img, MagickFormat formato, string salida, uint calidad = 92)
    {
        if (formato is MagickFormat.Jpeg or MagickFormat.Bmp && img.HasAlpha)
        {
            // JPG no tiene transparencia: sin esto lo transparente sale negro
            img.BackgroundColor = MagickColors.White;
            img.Alpha(AlphaAction.Remove);
        }

        if (formato is MagickFormat.Jpeg or MagickFormat.WebP) img.Quality = calidad;

        if (formato == MagickFormat.Ico)
        {
            // Un icono es cuadrado y de 256 como mucho. Se centra sobre fondo transparente
            // en vez de deformarlo, y se guardan los tamaños que usa Windows.
            var lado = Math.Max(img.Width, img.Height);
            if (img.Width != img.Height)
            {
                img.BackgroundColor = MagickColors.Transparent;
                img.Extent(lado, lado, Gravity.Center);
            }
            if (lado > 256) img.Resize(256, 256);
            img.Settings.SetDefine(MagickFormat.Ico, "auto-resize", "256,128,64,48,32,16");
        }

        img.Write(salida, formato);
    }
}
