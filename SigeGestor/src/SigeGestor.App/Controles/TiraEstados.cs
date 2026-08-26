using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using SigeGestor.Core.Operaciones;

namespace SigeGestor.App.Controles;

/// <summary>
/// Una celda por entrada, coloreada según cómo ha ido: verde con datos, gris sin datos, roja
/// fallida, tinta la que se está procesando, hueca la pendiente.
///
/// Se dibuja en OnRender y no con un ItemsControl porque con 216 entradas —o 2.000— crear un
/// visual por celda y volver a plantillarlo en cada actualización de progreso es caro, y el
/// progreso se refresca en cada entrada.
///
/// Cuando hay más entradas que píxeles disponibles, varias entradas comparten celda y manda
/// la peor: un fallo nunca se pierde por falta de sitio.
/// </summary>
public sealed class TiraEstados : FrameworkElement
{
    private const double RadioEsquina = 3;

    public static readonly DependencyProperty EstadosProperty = DependencyProperty.Register(
        nameof(Estados), typeof(IReadOnlyList<EstadoEntrada>), typeof(TiraEstados),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public IReadOnlyList<EstadoEntrada>? Estados
    {
        get => (IReadOnlyList<EstadoEntrada>?)GetValue(EstadosProperty);
        set => SetValue(EstadosProperty, value);
    }

    protected override void OnRender(DrawingContext dc)
    {
        var ancho = ActualWidth;
        var alto = ActualHeight;
        if (ancho <= 1 || alto <= 1) return;

        var fondo = Pincel("HairBrush");
        var recorte = new RectangleGeometry(new Rect(0, 0, ancho, alto), RadioEsquina, RadioEsquina);
        recorte.Freeze();

        dc.PushClip(recorte);
        dc.DrawRectangle(fondo, null, new Rect(0, 0, ancho, alto));

        var estados = Estados;
        if (estados is not null && estados.Count > 0)
        {
            // Nunca menos de un píxel por celda: por debajo de eso no se vería nada.
            var celdas = Math.Min(estados.Count, Math.Max(1, (int)ancho));
            var anchoCelda = ancho / celdas;

            for (var c = 0; c < celdas; c++)
            {
                var desde = (int)Math.Floor(c * estados.Count / (double)celdas);
                var hasta = (int)Math.Floor((c + 1) * estados.Count / (double)celdas);
                if (hasta <= desde) hasta = desde + 1;

                var peor = PeorDe(estados, desde, Math.Min(hasta, estados.Count));
                if (peor == EstadoEntrada.Pendiente) continue;

                var pincel = PincelDe(peor);
                if (pincel is null) continue;

                // Se solapa un pelo cada celda con la siguiente para que no queden costuras
                // blancas por el redondeo de subpíxel.
                dc.DrawRectangle(pincel, null,
                    new Rect(c * anchoCelda, 0, anchoCelda + 0.6, alto));
            }
        }

        dc.Pop();
    }

    /// <summary>
    /// De las entradas que comparten celda, manda la más grave. El orden importa: un fallo
    /// tapa a un acierto, nunca al revés.
    /// </summary>
    private static EstadoEntrada PeorDe(IReadOnlyList<EstadoEntrada> estados, int desde, int hasta)
    {
        var peor = EstadoEntrada.Pendiente;
        for (var i = desde; i < hasta; i++)
        {
            var e = estados[i];
            if (Gravedad(e) > Gravedad(peor)) peor = e;
        }
        return peor;
    }

    private static int Gravedad(EstadoEntrada estado) => estado switch
    {
        EstadoEntrada.Pendiente => 0,
        EstadoEntrada.ConDatos => 1,
        EstadoEntrada.SinDatos => 2,
        EstadoEntrada.EnCurso => 3,
        EstadoEntrada.Fallo => 4,
        _ => 0
    };

    private static Brush? PincelDe(EstadoEntrada estado) => estado switch
    {
        EstadoEntrada.ConDatos => Pincel("RepMarkBrush"),
        EstadoEntrada.SinDatos => Pincel("UatMarkBrush"),
        EstadoEntrada.Fallo => Pincel("BadBrush"),
        EstadoEntrada.EnCurso => Pincel("MarcaBrush"),
        _ => null
    };

    private static Brush Pincel(string clave) =>
        Application.Current?.TryFindResource(clave) as Brush ?? Brushes.Transparent;
}
