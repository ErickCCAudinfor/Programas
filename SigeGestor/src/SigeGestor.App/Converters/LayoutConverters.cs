using System;
using System.Globalization;
using System.Windows.Data;

namespace SigeGestor.App.Converters;

/// <summary>
/// Ancho disponible a número de columnas de la rejilla de operaciones.
///
/// Se usa contra el ActualWidth del contenedor, no contra el de la ventana: la barra lateral
/// mide 236 y el ScrollViewer tiene su propio relleno, así que la ventana no dice cuánto
/// espacio queda de verdad.
///
/// Los cortes salen del ancho mínimo con el que una tarjeta sigue siendo legible, ~330: la
/// descripción son dos o tres líneas y por debajo de eso pasa a cinco. Con la ventana en su
/// mínimo (880) quedan unos 590 útiles, que es una sola columna a propósito; dos de 295
/// entrarían pero no se leerían.
/// </summary>
public sealed class AnchoAColumnasConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var ancho = value is double d ? d : 0d;

        // Antes de la primera medida el ancho es 0 o NaN. Devolver 0 columnas deja el
        // UniformGrid sin repartir y las tarjetas se apilan sin ancho: una columna es el
        // valor seguro.
        if (double.IsNaN(ancho) || ancho <= 0) return 1;

        if (ancho < 700) return 1;
        if (ancho < 1120) return 2;
        return 3;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}
