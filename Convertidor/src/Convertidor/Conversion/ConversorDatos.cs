using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using ClosedXML.Excel;

namespace Convertidor.Conversion;

/// <summary>
/// Datos en forma de tabla: CSV, TSV, JSON, XML y Excel (.xlsx) entre sí.
///
/// Todo pasa por una tabla intermedia de textos (columnas + filas), así cada formato solo
/// necesita saber leer y escribir esa tabla, y no hay que escribir cada pareja a mano.
///
/// Criterios fijos, pensados para abrirlo luego en un Excel en español:
///   - El CSV se escribe con punto y coma y en UTF-8 con BOM (sin el BOM Excel rompe las tildes).
///   - Del .xlsx solo se lee la primera hoja.
///   - Al escribir el .xlsx, lo que es claramente un número va como número. Lo que empieza por
///     cero ("0021", un CUPS, un código postal) se deja como texto para no perder los ceros.
/// </summary>
public sealed class ConversorDatos : IConversor
{
    private static readonly HashSet<string> Entradas = new() { ".csv", ".tsv", ".json", ".xml", ".xlsx", ".xlsm" };

    private static readonly FormatoDestino[] Salidas =
    {
        new("xlsx", "Excel (.xlsx)", ".xlsx"),
        new("csv",  "CSV (;)",       ".csv"),
        new("tsv",  "TSV (tabulador)", ".tsv"),
        new("json", "JSON",          ".json"),
        new("xml",  "XML",           ".xml"),
    };

    public string Familia => "Datos";

    public bool Admite(string extension) => Entradas.Contains(extension);

    public IReadOnlyList<FormatoDestino> Destinos(string ruta)
    {
        var ext = Path.GetExtension(ruta).ToLowerInvariant();
        if (ext == ".xlsm") ext = ".xlsx";
        return Salidas.Where(s => s.Extension != ext).ToArray();
    }

    public string Convertir(string ruta, FormatoDestino destino, string carpetaSalida, ContextoConversion contexto)
    {
        var tabla = Leer(ruta);
        var salida = RutaSalida.Archivo(carpetaSalida, RutaSalida.NombreBase(ruta), destino.Extension);

        switch (destino.Clave)
        {
            case "csv": EscribirSeparado(tabla, salida, ';'); break;
            case "tsv": EscribirSeparado(tabla, salida, '\t'); break;
            case "json": EscribirJson(tabla, salida); break;
            case "xml": EscribirXml(tabla, salida); break;
            case "xlsx": EscribirExcel(tabla, salida); break;
            default: throw new NotSupportedException(destino.Nombre);
        }
        return salida;
    }

    // =====================================================================================
    // Tabla intermedia
    // =====================================================================================

    private sealed class Tabla
    {
        public List<string> Columnas { get; } = new();
        public List<List<string>> Filas { get; } = new();

        public int Columna(string nombre)
        {
            var i = Columnas.IndexOf(nombre);
            if (i >= 0) return i;
            Columnas.Add(nombre);
            foreach (var f in Filas) f.Add("");
            return Columnas.Count - 1;
        }

        public List<string> NuevaFila()
        {
            var f = Enumerable.Repeat("", Columnas.Count).ToList();
            Filas.Add(f);
            return f;
        }

        public void Poner(List<string> fila, string columna, string valor)
        {
            var i = Columna(columna);
            while (fila.Count < Columnas.Count) fila.Add("");
            fila[i] = valor;
        }
    }

    private static Tabla Leer(string ruta) => Path.GetExtension(ruta).ToLowerInvariant() switch
    {
        ".csv" => LeerSeparado(ruta, null),
        ".tsv" => LeerSeparado(ruta, '\t'),
        ".json" => LeerJson(ruta),
        ".xml" => LeerXml(ruta),
        ".xlsx" or ".xlsm" => LeerExcel(ruta),
        _ => throw new NotSupportedException(Path.GetExtension(ruta)),
    };

    // =====================================================================================
    // CSV / TSV
    // =====================================================================================

    private static Tabla LeerSeparado(string ruta, char? separador)
    {
        var texto = Codificacion.LeerTexto(ruta);
        var sep = separador ?? AdivinarSeparador(texto);
        var lineas = PartirCsv(texto, sep);

        var tabla = new Tabla();
        if (lineas.Count == 0) return tabla;

        // Cabeceras repetidas o vacías se renombran: en JSON y XML el nombre es la clave
        var usados = new HashSet<string>();
        for (var i = 0; i < lineas[0].Count; i++)
        {
            var nombre = string.IsNullOrWhiteSpace(lineas[0][i]) ? $"Columna{i + 1}" : lineas[0][i].Trim();
            var unico = nombre;
            for (var n = 2; !usados.Add(unico); n++) unico = $"{nombre}_{n}";
            tabla.Columnas.Add(unico);
        }

        foreach (var linea in lineas.Skip(1))
        {
            if (linea.Count == 1 && linea[0].Length == 0) continue; // línea en blanco
            while (linea.Count > tabla.Columnas.Count) tabla.Columna($"Columna{tabla.Columnas.Count + 1}");
            while (linea.Count < tabla.Columnas.Count) linea.Add("");
            tabla.Filas.Add(linea);
        }
        return tabla;
    }

    /// <summary>El que más aparece en la primera línea, contando solo fuera de comillas.</summary>
    private static char AdivinarSeparador(string texto)
    {
        var cuentas = new Dictionary<char, int> { [';'] = 0, [','] = 0, ['\t'] = 0, ['|'] = 0 };
        var dentro = false;
        foreach (var c in texto)
        {
            if (c == '"') dentro = !dentro;
            else if (!dentro && (c == '\n' || c == '\r')) break;
            else if (!dentro && cuentas.ContainsKey(c)) cuentas[c]++;
        }
        var mejor = cuentas.MaxBy(kv => kv.Value);
        return mejor.Value > 0 ? mejor.Key : ';';
    }

    /// <summary>CSV de verdad: comillas, comillas dobladas y saltos de línea dentro de un campo.</summary>
    private static List<List<string>> PartirCsv(string texto, char sep)
    {
        var filas = new List<List<string>>();
        var fila = new List<string>();
        var campo = new StringBuilder();
        var comillas = false;

        for (var i = 0; i < texto.Length; i++)
        {
            var c = texto[i];
            if (comillas)
            {
                if (c == '"' && i + 1 < texto.Length && texto[i + 1] == '"') { campo.Append('"'); i++; }
                else if (c == '"') comillas = false;
                else campo.Append(c);
            }
            // Solo abre comillas la que va al principio del campo. Una suelta en medio
            // (Peñalver "el bueno", sin entrecomillar el campo entero) es texto normal.
            else if (c == '"' && campo.Length == 0) comillas = true;
            else if (c == sep) { fila.Add(campo.ToString()); campo.Clear(); }
            else if (c == '\r' || c == '\n')
            {
                if (c == '\r' && i + 1 < texto.Length && texto[i + 1] == '\n') i++;
                fila.Add(campo.ToString()); campo.Clear();
                filas.Add(fila); fila = new List<string>();
            }
            else campo.Append(c);
        }

        if (campo.Length > 0 || fila.Count > 0)
        {
            fila.Add(campo.ToString());
            filas.Add(fila);
        }
        return filas;
    }

    private static void EscribirSeparado(Tabla t, string salida, char sep)
    {
        string Campo(string v) =>
            v.IndexOfAny(new[] { sep, '"', '\r', '\n' }) >= 0 ? "\"" + v.Replace("\"", "\"\"") + "\"" : v;

        using var w = new StreamWriter(salida, false, new UTF8Encoding(true));
        w.Write(string.Join(sep, t.Columnas.Select(Campo)));
        w.Write("\r\n");
        foreach (var f in t.Filas)
        {
            w.Write(string.Join(sep, f.Select(Campo)));
            w.Write("\r\n");
        }
    }

    // =====================================================================================
    // JSON
    // =====================================================================================

    private static Tabla LeerJson(string ruta)
    {
        using var doc = JsonDocument.Parse(Codificacion.LeerTexto(ruta), new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        });

        // Se busca la lista de registros: o es la raíz, o la primera propiedad que sea una lista
        // de objetos ({"datos": [ ... ]}), o la raíz es un objeto suelto y es una fila.
        var raiz = doc.RootElement;
        IEnumerable<JsonElement> registros = raiz.ValueKind switch
        {
            JsonValueKind.Array => raiz.EnumerateArray(),
            JsonValueKind.Object => raiz.EnumerateObject()
                .Select(p => p.Value)
                .FirstOrDefault(v => v.ValueKind == JsonValueKind.Array
                                     && v.EnumerateArray().Any(e => e.ValueKind == JsonValueKind.Object)) is { ValueKind: JsonValueKind.Array } lista
                ? lista.EnumerateArray()
                : new[] { raiz },
            _ => new[] { raiz },
        };

        var tabla = new Tabla();
        foreach (var r in registros)
        {
            var fila = tabla.NuevaFila();
            if (r.ValueKind == JsonValueKind.Object)
                foreach (var p in r.EnumerateObject())
                    tabla.Poner(fila, p.Name, ValorJson(p.Value));
            else
                tabla.Poner(fila, "valor", ValorJson(r));
        }
        return tabla;
    }

    private static string ValorJson(JsonElement v) => v.ValueKind switch
    {
        JsonValueKind.String => v.GetString() ?? "",
        JsonValueKind.Null or JsonValueKind.Undefined => "",
        _ => v.GetRawText(), // números, true/false, y objetos o listas anidados tal cual
    };

    private static void EscribirJson(Tabla t, string salida)
    {
        using var fs = File.Create(salida);
        using var w = new Utf8JsonWriter(fs, new JsonWriterOptions
        {
            Indented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // tildes legibles, no á
        });

        w.WriteStartArray();
        foreach (var f in t.Filas)
        {
            w.WriteStartObject();
            for (var i = 0; i < t.Columnas.Count; i++)
                w.WriteString(t.Columnas[i], i < f.Count ? f[i] : "");
            w.WriteEndObject();
        }
        w.WriteEndArray();
    }

    // =====================================================================================
    // XML
    // =====================================================================================

    private static Tabla LeerXml(string ruta)
    {
        var doc = XDocument.Load(ruta);
        var raiz = doc.Root ?? throw new InvalidOperationException("el XML está vacío");

        // <Datos><Filas><Fila/>...</Filas></Datos>: si la raíz solo tiene un hijo y ese hijo
        // es el que tiene la lista, se baja un nivel.
        var contenedor = raiz;
        while (contenedor.Elements().Count() == 1 && contenedor.Elements().First().Elements().Any(e => e.HasElements))
            contenedor = contenedor.Elements().First();

        var tabla = new Tabla();
        foreach (var registro in contenedor.Elements())
        {
            var fila = tabla.NuevaFila();
            foreach (var a in registro.Attributes().Where(a => !a.IsNamespaceDeclaration))
                tabla.Poner(fila, XmlConvert.DecodeName(a.Name.LocalName), a.Value);

            if (!registro.HasElements && !registro.HasAttributes)
                tabla.Poner(fila, XmlConvert.DecodeName(registro.Name.LocalName), registro.Value);

            foreach (var campo in registro.Elements())
                tabla.Poner(fila, XmlConvert.DecodeName(campo.Name.LocalName),
                    campo.HasElements ? string.Concat(campo.Nodes()) : campo.Value);
        }
        return tabla;
    }

    private static void EscribirXml(Tabla t, string salida)
    {
        // EncodeLocalName deja "Fecha alta" como "Fecha_x0020_alta": feo pero es XML válido y al
        // leerlo de vuelta se recupera el nombre original.
        var nombres = t.Columnas.Select(c => XmlConvert.EncodeLocalName(c)!).ToArray();
        var raiz = new XElement("Datos",
            t.Filas.Select(f => new XElement("Fila",
                nombres.Select((n, i) => new XElement(n, i < f.Count ? f[i] : "")))));

        new XDocument(new XDeclaration("1.0", "utf-8", null), raiz).Save(salida);
    }

    // =====================================================================================
    // Excel
    // =====================================================================================

    private static Tabla LeerExcel(string ruta)
    {
        // FileShare.ReadWrite para poder leerlo aunque esté abierto en Excel
        using var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var libro = new XLWorkbook(fs);
        var hoja = libro.Worksheets.First();
        var rango = hoja.RangeUsed();

        var tabla = new Tabla();
        if (rango == null) return tabla;

        var filas = rango.Rows().ToList();
        var usados = new HashSet<string>();
        var ancho = rango.ColumnCount();
        var c = 0;
        foreach (var celda in filas[0].Cells(1, ancho))
        {
            c++;
            var nombre = celda.GetFormattedString().Trim();
            if (nombre.Length == 0) nombre = $"Columna{c}";
            var unico = nombre;
            for (var n = 2; !usados.Add(unico); n++) unico = $"{nombre}_{n}";
            tabla.Columnas.Add(unico);
        }

        foreach (var fila in filas.Skip(1))
            tabla.Filas.Add(fila.Cells(1, ancho).Select(x => x.GetFormattedString()).ToList());

        return tabla;
    }

    private static readonly Regex Numero = new(@"^-?(0|[1-9]\d{0,14})([.,]\d+)?$", RegexOptions.Compiled);

    private static void EscribirExcel(Tabla t, string salida)
    {
        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add("Datos");

        for (var c = 0; c < t.Columnas.Count; c++)
            hoja.Cell(1, c + 1).Value = t.Columnas[c];

        for (var r = 0; r < t.Filas.Count; r++)
        {
            var fila = t.Filas[r];
            for (var c = 0; c < fila.Count; c++)
            {
                var v = fila[c];
                var celda = hoja.Cell(r + 2, c + 1);
                if (Numero.IsMatch(v)
                    && double.TryParse(v.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                    celda.Value = d;
                else
                    celda.Value = v;
            }
        }

        if (t.Columnas.Count > 0)
        {
            var cabecera = hoja.Range(1, 1, 1, t.Columnas.Count);
            cabecera.Style.Font.Bold = true;
            cabecera.SetAutoFilter();
            hoja.SheetView.FreezeRows(1);
            hoja.Columns(1, t.Columnas.Count).AdjustToContents(1, Math.Min(t.Filas.Count + 1, 500));
        }

        libro.SaveAs(salida);
    }
}
