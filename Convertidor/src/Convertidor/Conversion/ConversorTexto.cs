using System.Text;

namespace Convertidor.Conversion;

/// <summary>
/// Cambia la codificación de un archivo de texto sin tocar su extensión.
/// El resultado se llama "nombre (UTF-8).sql", junto al original, que no se modifica.
/// </summary>
public sealed class ConversorTexto : IConversor
{
    private static readonly HashSet<string> Entradas = new()
    {
        ".txt", ".csv", ".tsv", ".sql", ".xml", ".json", ".log", ".md", ".html", ".htm",
        ".ini", ".config", ".cs", ".vb", ".ps1", ".bat", ".cmd", ".js", ".css", ".yml", ".yaml",
    };

    private static readonly string[] Todas =
        { Codificacion.Utf8, Codificacion.Utf8Bom, Codificacion.Ansi, Codificacion.Utf16 };

    public string Familia => "Texto";

    public bool Admite(string extension) => Entradas.Contains(extension);

    public IReadOnlyList<FormatoDestino> Destinos(string ruta)
    {
        var actual = Codificacion.Detectar(ruta, maxBytes: 1 << 20);
        var ext = Path.GetExtension(ruta);
        return Todas
            .Where(c => c != actual)
            .Select(c => new FormatoDestino(c, Codificacion.Nombre(c), ext))
            .ToArray();
    }

    public string Convertir(string ruta, FormatoDestino destino, string carpetaSalida, ContextoConversion contexto)
    {
        var texto = Codificacion.LeerTexto(ruta);
        var codificador = Codificacion.ParaEscribir(destino.Clave);

        byte[] cuerpo;
        try
        {
            cuerpo = codificador.GetBytes(texto);
        }
        catch (EncoderFallbackException ex)
        {
            throw new InvalidOperationException(
                $"tiene el carácter «{ex.CharUnknown}» que no existe en {destino.Nombre}");
        }

        var etiqueta = Codificacion.Nombre(destino.Clave).Replace(" (Windows-1252)", "");
        var salida = RutaSalida.Archivo(carpetaSalida, $"{RutaSalida.NombreBase(ruta)} ({etiqueta})", destino.Extension);

        using var fs = File.Create(salida);
        fs.Write(codificador.GetPreamble());
        fs.Write(cuerpo);
        return salida;
    }
}
