using System.Text;

namespace Convertidor.Conversion;

/// <summary>
/// Saber en qué codificación viene un texto.
///
/// Es lo que nos pasa con los .sql que SSMS guarda en ANSI: abiertos como UTF-8 las tildes
/// salen rotas. Primero se mira el BOM; si no hay, se intenta leer como UTF-8 estricto, y si
/// eso falla es que viene en ANSI (Windows-1252).
/// </summary>
public static class Codificacion
{
    static Codificacion() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    public const string Utf8 = "utf8";
    public const string Utf8Bom = "utf8bom";
    public const string Ansi = "ansi";
    public const string Utf16 = "utf16";

    public static Encoding Ansi1252 => Encoding.GetEncoding(1252);

    /// <param name="maxBytes">Para decidir qué ofrecer basta con el principio del archivo.</param>
    public static string Detectar(string ruta, int maxBytes = int.MaxValue)
    {
        byte[] bytes;
        using (var fs = File.OpenRead(ruta))
        {
            var n = (int)Math.Min(fs.Length, maxBytes);
            bytes = new byte[n];
            fs.ReadExactly(bytes, 0, n);
        }
        return Detectar(bytes, recortado: bytes.Length == maxBytes);
    }

    public static string Detectar(byte[] b, bool recortado = false)
    {
        if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) return Utf8Bom;
        if (b.Length >= 2 && ((b[0] == 0xFF && b[1] == 0xFE) || (b[0] == 0xFE && b[1] == 0xFF))) return Utf16;

        // Si el trozo está cortado puede partir una letra de UTF-8 por la mitad: se quitan
        // los últimos bytes para no confundir eso con ANSI.
        var largo = recortado ? Math.Max(0, b.Length - 4) : b.Length;
        try
        {
            new UTF8Encoding(false, true).GetString(b, 0, largo);
            return Utf8;
        }
        catch (DecoderFallbackException)
        {
            return Ansi;
        }
    }

    public static string LeerTexto(string ruta) => LeerTexto(ruta, out _);

    public static string LeerTexto(string ruta, out string codificacion)
    {
        var bytes = File.ReadAllBytes(ruta);
        codificacion = Detectar(bytes);
        return codificacion switch
        {
            Utf8Bom => Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3),
            Utf16 => bytes[0] == 0xFF
                ? Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2)
                : Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2),
            Ansi => Ansi1252.GetString(bytes),
            _ => Encoding.UTF8.GetString(bytes),
        };
    }

    /// <summary>
    /// Codificador que falla en vez de poner "?" a escondidas cuando una letra no cabe
    /// (pasa al bajar de UTF-8 a ANSI con caracteres como "→" o emojis).
    /// </summary>
    public static Encoding ParaEscribir(string clave) => clave switch
    {
        Utf8 => new UTF8Encoding(false, true),
        Utf8Bom => new UTF8Encoding(true, true),
        Utf16 => new UnicodeEncoding(false, true, true),
        Ansi => Encoding.GetEncoding(1252, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback),
        _ => throw new NotSupportedException(clave),
    };

    public static string Nombre(string clave) => clave switch
    {
        Utf8 => "UTF-8",
        Utf8Bom => "UTF-8 con BOM",
        Utf16 => "UTF-16",
        Ansi => "ANSI (Windows-1252)",
        _ => clave,
    };
}
