using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SigeGestor.App.Controles;

/// <summary>
/// Sparkline: área tenue, línea y punto final destacado.
///
/// Se dibuja en OnRender en lugar de componerlo con Path y un conversor porque así el
/// relleno y la línea comparten el mismo recorrido calculado una sola vez, y no hay que
/// pelearse con el escalado de un Stretch.
///
/// El eje Y se ajusta al mínimo y al máximo de la serie, no a cero: en cifras como la
/// duración media, que se mueven poco, arrancar en cero dejaría una línea plana que no
/// dice nada.
/// </summary>
public sealed class Sparkline : FrameworkElement
{
    /// <summary>Margen interior para que el trazo y el punto final no se recorten.</summary>
    private const double Margen = 3.5;

    private const double RadioPunto = 2.6;

    public static readonly DependencyProperty ValoresProperty = DependencyProperty.Register(
        nameof(Valores), typeof(IReadOnlyList<double>), typeof(Sparkline),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty TrazoProperty = DependencyProperty.Register(
        nameof(Trazo), typeof(Brush), typeof(Sparkline),
        new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty RellenoProperty = DependencyProperty.Register(
        nameof(Relleno), typeof(Brush), typeof(Sparkline),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty PuntoProperty = DependencyProperty.Register(
        nameof(Punto), typeof(Brush), typeof(Sparkline),
        new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty GrosorProperty = DependencyProperty.Register(
        nameof(Grosor), typeof(double), typeof(Sparkline),
        new FrameworkPropertyMetadata(1.5, FrameworkPropertyMetadataOptions.AffectsRender));

    public IReadOnlyList<double>? Valores
    {
        get => (IReadOnlyList<double>?)GetValue(ValoresProperty);
        set => SetValue(ValoresProperty, value);
    }

    public Brush? Trazo
    {
        get => (Brush?)GetValue(TrazoProperty);
        set => SetValue(TrazoProperty, value);
    }

    public Brush? Relleno
    {
        get => (Brush?)GetValue(RellenoProperty);
        set => SetValue(RellenoProperty, value);
    }

    public Brush? Punto
    {
        get => (Brush?)GetValue(PuntoProperty);
        set => SetValue(PuntoProperty, value);
    }

    public double Grosor
    {
        get => (double)GetValue(GrosorProperty);
        set => SetValue(GrosorProperty, value);
    }

    protected override void OnRender(DrawingContext dc)
    {
        var valores = Valores;
        if (valores is null || valores.Count < 2) return;

        var ancho = ActualWidth;
        var alto = ActualHeight;
        if (ancho <= Margen * 2 || alto <= Margen * 2) return;

        var minimo = valores.Min();
        var maximo = valores.Max();
        var recorrido = maximo - minimo;

        var x0 = Margen;
        var anchoUtil = ancho - Margen * 2;
        var y0 = Margen;
        var altoUtil = alto - Margen * 2;

        var puntos = new Point[valores.Count];
        for (var i = 0; i < valores.Count; i++)
        {
            var x = x0 + anchoUtil * i / (valores.Count - 1);

            // Serie constante: se dibuja centrada en vez de pegada a un borde.
            var proporcion = recorrido <= double.Epsilon ? 0.5 : (valores[i] - minimo) / recorrido;
            var y = y0 + altoUtil * (1 - proporcion);

            puntos[i] = new Point(x, y);
        }

        if (Relleno is not null)
        {
            var area = new StreamGeometry();
            using (var ctx = area.Open())
            {
                ctx.BeginFigure(new Point(puntos[0].X, alto), isFilled: true, isClosed: true);
                ctx.LineTo(puntos[0], isStroked: false, isSmoothJoin: false);
                for (var i = 1; i < puntos.Length; i++)
                {
                    ctx.LineTo(puntos[i], isStroked: false, isSmoothJoin: false);
                }
                ctx.LineTo(new Point(puntos[^1].X, alto), isStroked: false, isSmoothJoin: false);
            }
            area.Freeze();
            dc.DrawGeometry(Relleno, null, area);
        }

        if (Trazo is not null)
        {
            var linea = new StreamGeometry();
            using (var ctx = linea.Open())
            {
                ctx.BeginFigure(puntos[0], isFilled: false, isClosed: false);
                for (var i = 1; i < puntos.Length; i++)
                {
                    ctx.LineTo(puntos[i], isStroked: true, isSmoothJoin: true);
                }
            }
            linea.Freeze();

            var pluma = new Pen(Trazo, Grosor)
            {
                LineJoin = PenLineJoin.Round,
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            pluma.Freeze();
            dc.DrawGeometry(null, pluma, linea);
        }

        if (Punto is not null)
        {
            dc.DrawEllipse(Punto, null, puntos[^1], RadioPunto, RadioPunto);
        }
    }
}
