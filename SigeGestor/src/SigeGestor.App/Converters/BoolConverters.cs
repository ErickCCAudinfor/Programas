using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SigeGestor.App.Converters;

/// <summary>Invierte un booleano. Para IsEnabled="{Binding Validando}" al revés.</summary>
public sealed class BoolNegadoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : true;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : false;
}

/// <summary>Booleano a Visibility, pero al revés: True oculta.</summary>
public sealed class BoolAVisibilidadNegadaConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && b ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility v && v != Visibility.Visible;
}
