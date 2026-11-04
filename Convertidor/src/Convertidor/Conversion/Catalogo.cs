namespace Convertidor.Conversion;

/// <summary>
/// Reparte cada archivo entre los conversores.
///
/// Un mismo archivo puede tenerlo más de uno: un .csv lo lee el conversor de datos
/// (a JSON, XML, Excel...) y también el de texto (cambiar la codificación). Se juntan los
/// formatos de todos, y si dos ofrecen la misma clave gana el primero de la lista.
/// Por eso el orden de abajo importa: el de datos va antes que Excel para que un .xlsx
/// pase a CSV con ClosedXML (punto y coma y UTF-8, como lo queremos) y no con el CSV de Excel.
/// </summary>
public static class Catalogo
{
    private static readonly IConversor[] Conversores =
    {
        new ConversorImagen(),
        new ConversorPdf(),   // antes que Word: el PDF sale como "PDF" en la columna Tipo
        new ConversorDatos(),
        new ConversorWord(),
        new ConversorExcel(),
        new ConversorPowerPoint(),
        new ConversorTexto(),
    };

    private static string Ext(string ruta) => Path.GetExtension(ruta).ToLowerInvariant();

    public static string? Familia(string ruta) =>
        Conversores.FirstOrDefault(c => c.Admite(Ext(ruta)))?.Familia;

    public static IReadOnlyList<FormatoDestino> Destinos(string ruta)
    {
        var ext = Ext(ruta);
        var vistos = new HashSet<string>();
        var lista = new List<FormatoDestino>();

        foreach (var c in Conversores.Where(c => c.Admite(ext)))
        {
            IReadOnlyList<FormatoDestino> suyos;
            try { suyos = c.Destinos(ruta); }
            catch { continue; } // un archivo que no se deja leer no debe tumbar la ventana

            foreach (var d in suyos)
                if (vistos.Add(d.Clave))
                    lista.Add(d);
        }
        return lista;
    }

    /// <summary>Devuelve null si el archivo no se puede pasar a ese formato.</summary>
    public static (IConversor Conversor, FormatoDestino Destino)? Resolver(string ruta, string clave)
    {
        var ext = Ext(ruta);
        foreach (var c in Conversores.Where(c => c.Admite(ext)))
        {
            var d = c.Destinos(ruta).FirstOrDefault(x => x.Clave == clave);
            if (d != null) return (c, d);
        }
        return null;
    }
}
