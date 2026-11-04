using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using ColorWinRT = Windows.UI.Color;
using RectWinRT = Windows.Foundation.Rect;

namespace Convertidor.Conversion.Paginas;

/// <summary>
/// PDF con el lector que trae Windows 10/11 (Windows.Data.Pdf, el mismo motor que usa Edge).
///
/// Se eligió este y no Ghostscript (licencia AGPL, y hay que instalarlo) ni PDFium (más peso en
/// el .exe). Además dibuja en vectorial: una zona pequeña sacada a 600 ppp sale nítida, no es
/// una ampliación de la página.
/// </summary>
public sealed class FuentePdf : IFuentePaginas
{
    private readonly PdfDocument _doc;
    private readonly (double, double)[] _tamanos;

    private FuentePdf(string ruta, PdfDocument doc)
    {
        Ruta = ruta;
        _doc = doc;
        _tamanos = new (double, double)[doc.PageCount];
        for (uint i = 0; i < doc.PageCount; i++)
        {
            using var p = doc.GetPage(i);
            _tamanos[i] = (p.Size.Width, p.Size.Height);
        }
    }

    public static async Task<FuentePdf> Abrir(string ruta)
    {
        var archivo = await StorageFile.GetFileFromPathAsync(Path.GetFullPath(ruta));
        try
        {
            return new FuentePdf(ruta, await PdfDocument.LoadFromFileAsync(archivo));
        }
        catch (Exception ex) when ((uint)ex.HResult == 0x8007052B) // contraseña incorrecta
        {
            throw new InvalidOperationException("el PDF está protegido con contraseña");
        }
    }

    public string Ruta { get; }
    public int Paginas => _tamanos.Length;
    public (double Ancho, double Alto) Tamano(int pagina) => _tamanos[pagina];
    public double PppPropuesta => 200;
    public bool AdmiteFondoTransparente => true;

    public async Task<byte[]> DibujarPng(int pagina, Region region, int pxAncho, int pxAlto, bool transparente)
    {
        using var p = _doc.GetPage((uint)pagina);
        var (w, h) = _tamanos[pagina];

        var opciones = new PdfPageRenderOptions
        {
            // SourceRect va en las mismas unidades que PdfPage.Size (DIP)
            SourceRect = new RectWinRT(region.X * w, region.Y * h, region.Ancho * w, region.Alto * h),
            DestinationWidth = (uint)pxAncho,
            DestinationHeight = (uint)pxAlto,
            BitmapEncoderId = BitmapEncoder.PngEncoderId,
            BackgroundColor = transparente
                ? ColorWinRT.FromArgb(0, 255, 255, 255)
                : ColorWinRT.FromArgb(255, 255, 255, 255),
        };

        using var flujo = new InMemoryRandomAccessStream();
        await p.RenderToStreamAsync(flujo, opciones);

        var bytes = new byte[flujo.Size];
        using var lector = new DataReader(flujo.GetInputStreamAt(0));
        await lector.LoadAsync((uint)flujo.Size);
        lector.ReadBytes(bytes);
        return bytes;
    }

    public void Dispose() { } // PdfDocument no tiene Dispose; suelta el archivo al recogerse
}
