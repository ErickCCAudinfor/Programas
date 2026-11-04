namespace Convertidor.Conversion;

/// <summary>
/// Elige el nombre del resultado sin pisar nunca nada: si "informe.pdf" ya existe,
/// el nuevo será "informe (1).pdf".
/// </summary>
public static class RutaSalida
{
    public static string Archivo(string carpeta, string nombreBase, string extension)
    {
        var ruta = Path.Combine(carpeta, nombreBase + extension);
        for (var i = 1; File.Exists(ruta) || Directory.Exists(ruta); i++)
            ruta = Path.Combine(carpeta, $"{nombreBase} ({i}){extension}");
        return ruta;
    }

    public static string Carpeta(string carpeta, string nombre)
    {
        var ruta = Path.Combine(carpeta, nombre);
        for (var i = 1; File.Exists(ruta) || Directory.Exists(ruta); i++)
            ruta = Path.Combine(carpeta, $"{nombre} ({i})");
        Directory.CreateDirectory(ruta);
        return ruta;
    }

    public static string NombreBase(string ruta) => Path.GetFileNameWithoutExtension(ruta);
}
