using ImageMagick;

namespace Convertidor.Conversion.Paginas;

/// <summary>
/// Una imagen vista como "páginas". Un TIFF de varias páginas da una por página; en el resto
/// (un .ico con varios tamaños, un GIF animado) se coge una sola, la más grande.
/// </summary>
public sealed class FuenteImagen : IFuentePaginas
{
    private readonly List<MagickImage> _paginas = new();
    private readonly double _pppNativa;
    private readonly object _cerrojo = new();

    public FuenteImagen(string ruta)
    {
        Ruta = ruta;
        using var todas = new MagickImageCollection(ruta);
        var ext = Path.GetExtension(ruta).ToLowerInvariant();

        if (ext is ".tif" or ".tiff")
            foreach (var i in todas) _paginas.Add((MagickImage)i.Clone());
        else
            _paginas.Add((MagickImage)todas.OrderByDescending(i => (long)i.Width * i.Height).First().Clone());

        foreach (var p in _paginas) p.ResetPage();

        // La resolución que diga la propia imagen; sin dato, 96 (lo normal en pantalla)
        var d = _paginas[0].Density;
        _pppNativa = d.X > 1
            ? (d.Units == DensityUnit.PixelsPerCentimeter ? d.X * 2.54 : d.X)
            : 96;
    }

    public string Ruta { get; }
    public int Paginas => _paginas.Count;
    public double PppPropuesta => Math.Round(_pppNativa);
    public bool AdmiteFondoTransparente => false;

    public (double Ancho, double Alto) Tamano(int pagina)
    {
        var p = _paginas[pagina];
        return (p.Width * 96.0 / _pppNativa, p.Height * 96.0 / _pppNativa);
    }

    public Task<byte[]> DibujarPng(int pagina, Region region, int pxAncho, int pxAlto, bool transparente) =>
        Task.Run(() =>
        {
            MagickImage img;
            lock (_cerrojo) img = (MagickImage)_paginas[pagina].Clone();
            using (img)
            {
                var x = (int)Math.Round(region.X * img.Width);
                var y = (int)Math.Round(region.Y * img.Height);
                var w = (uint)Math.Max(1, Math.Round(region.Ancho * img.Width));
                var h = (uint)Math.Max(1, Math.Round(region.Alto * img.Height));
                if (region != Region.Entera)
                {
                    img.Crop(new MagickGeometry(x, y, w, h));
                    img.ResetPage();
                }

                if (img.Width != pxAncho || img.Height != pxAlto)
                    img.Resize(new MagickGeometry((uint)pxAncho, (uint)pxAlto) { IgnoreAspectRatio = true });

                return img.ToByteArray(MagickFormat.Png);
            }
        });

    public void Dispose()
    {
        foreach (var p in _paginas) p.Dispose();
        _paginas.Clear();
    }
}
