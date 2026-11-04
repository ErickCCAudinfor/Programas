namespace Convertidor.Conversion;

/// <summary>
/// Word: documentos, y también PDF a Word (Word reconstruye el documento a partir del PDF;
/// con PDF escaneados o maquetaciones complicadas el resultado puede necesitar retoques).
/// </summary>
public sealed class ConversorWord : IConversor
{
    private static readonly HashSet<string> Entradas = new()
    {
        ".doc", ".docx", ".docm", ".dot", ".dotx", ".rtf", ".odt", ".pdf", ".htm", ".html", ".mht", ".txt",
    };

    // Los números son las constantes WdSaveFormat de Word
    private static readonly (FormatoDestino Formato, int Codigo)[] Salidas =
    {
        (new("pdf",  "PDF",                    ".pdf"),  17),
        (new("docx", "Word (.docx)",           ".docx"), 16),
        (new("doc",  "Word 97-2003 (.doc)",    ".doc"),   0),
        (new("rtf",  "RTF",                    ".rtf"),   6),
        (new("odt",  "OpenDocument (.odt)",    ".odt"),  23),
        (new("html", "Página web (.html)",     ".html"), 10),
        (new("txt",  "Texto (.txt)",           ".txt"),   2),
        (new("xps",  "XPS",                    ".xps"),  18),
    };

    public string Familia => "Word";

    public bool Admite(string extension) => Entradas.Contains(extension);

    public IReadOnlyList<FormatoDestino> Destinos(string ruta)
    {
        var ext = Path.GetExtension(ruta).ToLowerInvariant();
        var salidas = Salidas.Select(s => s.Formato).Where(f => f.Extension != ext && !(ext == ".htm" && f.Clave == "html"));
        // El PDF entra en Word solo para sacarlo como documento editable
        if (ext == ".pdf") salidas = salidas.Where(f => f.Clave is "docx" or "doc" or "rtf" or "odt" or "txt");
        return salidas.ToArray();
    }

    public string Convertir(string ruta, FormatoDestino destino, string carpetaSalida, ContextoConversion contexto)
    {
        var codigo = Salidas.First(s => s.Formato.Clave == destino.Clave).Codigo;
        var salida = RutaSalida.Archivo(carpetaSalida, RutaSalida.NombreBase(ruta), destino.Extension);

        var word = contexto.Office.Word;
        dynamic? doc = null;
        try
        {
            doc = word.Documents.Open(FileName: ruta, ConfirmConversions: false, ReadOnly: true,
                AddToRecentFiles: false, Visible: false);

            if (destino.Clave == "txt")
                doc.SaveAs2(FileName: salida, FileFormat: codigo, Encoding: 65001); // UTF-8
            else
                doc.SaveAs2(FileName: salida, FileFormat: codigo);
        }
        finally
        {
            if (doc != null) { try { doc.Close(SaveChanges: 0); } catch { } SesionOffice.Liberar(doc); }
        }
        return salida;
    }
}

public sealed class ConversorExcel : IConversor
{
    private static readonly HashSet<string> Entradas = new() { ".xls", ".xlsx", ".xlsm", ".xlsb", ".ods" };

    // XlFileFormat de Excel. El PDF y el XPS no van por SaveAs sino por ExportAsFixedFormat.
    private static readonly (FormatoDestino Formato, int Codigo)[] Salidas =
    {
        (new("pdf",  "PDF",                       ".pdf"),  -1),
        (new("xlsx", "Excel (.xlsx)",             ".xlsx"), 51),
        (new("xls",  "Excel 97-2003 (.xls)",      ".xls"),  56),
        (new("ods",  "OpenDocument (.ods)",       ".ods"),  60),
        (new("csv",  "CSV (;)",                   ".csv"),   6),
        (new("html", "Página web (.html)",        ".html"), 44),
        (new("xps",  "XPS",                       ".xps"),  -2),
    };

    public string Familia => "Excel";

    public bool Admite(string extension) => Entradas.Contains(extension);

    public IReadOnlyList<FormatoDestino> Destinos(string ruta)
    {
        var ext = Path.GetExtension(ruta).ToLowerInvariant();
        return Salidas.Select(s => s.Formato).Where(f => f.Extension != ext).ToArray();
    }

    public string Convertir(string ruta, FormatoDestino destino, string carpetaSalida, ContextoConversion contexto)
    {
        var codigo = Salidas.First(s => s.Formato.Clave == destino.Clave).Codigo;
        var salida = RutaSalida.Archivo(carpetaSalida, RutaSalida.NombreBase(ruta), destino.Extension);

        var excel = contexto.Office.Excel;
        dynamic? libro = null;
        try
        {
            libro = excel.Workbooks.Open(Filename: ruta, UpdateLinks: 0, ReadOnly: true, AddToMru: false);

            if (codigo == -1) libro.ExportAsFixedFormat(Type: 0, Filename: salida);      // xlTypePDF
            else if (codigo == -2) libro.ExportAsFixedFormat(Type: 1, Filename: salida); // xlTypeXPS
            // Local: true para que el CSV salga con ";" y la coma decimal, como en un Excel en español.
            // Ojo: el CSV de Excel solo guarda la hoja activa.
            else if (destino.Clave == "csv") libro.SaveAs(Filename: salida, FileFormat: codigo, Local: true);
            else libro.SaveAs(Filename: salida, FileFormat: codigo);
        }
        finally
        {
            if (libro != null) { try { libro.Close(SaveChanges: false); } catch { } SesionOffice.Liberar(libro); }
        }
        return salida;
    }
}

public sealed class ConversorPowerPoint : IConversor
{
    private static readonly HashSet<string> Entradas = new() { ".ppt", ".pptx", ".pptm", ".pps", ".ppsx", ".odp" };

    // PpSaveAsFileType de PowerPoint. PNG y JPG se exportan diapositiva a diapositiva.
    private static readonly (FormatoDestino Formato, int Codigo)[] Salidas =
    {
        (new("pdf",  "PDF",                         ".pdf"),  32),
        (new("pptx", "PowerPoint (.pptx)",          ".pptx"), 24),
        (new("ppt",  "PowerPoint 97-2003 (.ppt)",   ".ppt"),   1),
        (new("odp",  "OpenDocument (.odp)",         ".odp"),  35),
        (new("png",  "PNG (una por diapositiva)",   ".png"),  -1),
        (new("jpg",  "JPG (una por diapositiva)",   ".jpg"),  -1),
        (new("xps",  "XPS",                         ".xps"),  33),
    };

    public string Familia => "PowerPoint";

    public bool Admite(string extension) => Entradas.Contains(extension);

    public IReadOnlyList<FormatoDestino> Destinos(string ruta)
    {
        var ext = Path.GetExtension(ruta).ToLowerInvariant();
        return Salidas.Select(s => s.Formato).Where(f => f.Extension != ext).ToArray();
    }

    public string Convertir(string ruta, FormatoDestino destino, string carpetaSalida, ContextoConversion contexto)
    {
        var codigo = Salidas.First(s => s.Formato.Clave == destino.Clave).Codigo;

        var ppt = contexto.Office.PowerPoint;
        dynamic? pres = null;
        try
        {
            // MsoTriState: -1 = sí, 0 = no
            pres = ppt.Presentations.Open(FileName: ruta, ReadOnly: -1, Untitled: 0, WithWindow: 0);

            if (codigo != -1)
            {
                var salida = RutaSalida.Archivo(carpetaSalida, RutaSalida.NombreBase(ruta), destino.Extension);
                pres.SaveAs(FileName: salida, FileFormat: codigo);
                return salida;
            }

            // Imágenes: una carpeta con una imagen por diapositiva, a 1920 de ancho
            var filtro = destino.Clave.ToUpperInvariant();
            var carpeta = RutaSalida.Carpeta(carpetaSalida, $"{RutaSalida.NombreBase(ruta)} ({filtro})");
            double ancho = pres.PageSetup.SlideWidth, alto = pres.PageSetup.SlideHeight;
            var px = 1920;
            var py = (int)Math.Round(px * alto / ancho);

            int total = pres.Slides.Count;
            for (var i = 1; i <= total; i++)
            {
                dynamic diapo = pres.Slides.Item(i);
                diapo.Export(Path.Combine(carpeta, $"Diapositiva {i:00}{destino.Extension}"), filtro, px, py);
                SesionOffice.Liberar(diapo);
            }
            return carpeta;
        }
        finally
        {
            if (pres != null) { try { pres.Close(); } catch { } SesionOffice.Liberar(pres); }
        }
    }
}
