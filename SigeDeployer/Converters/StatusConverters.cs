using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SigeDeployer.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRunning = value is bool b && b;
            return isRunning
                ? new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50))
                : new SolidColorBrush(Color.FromRgb(0xF4, 0x43, 0x36));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BoolToStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && b ? "▶" : "■";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && !b;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && !b;
    }
}
