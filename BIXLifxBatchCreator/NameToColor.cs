using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BIXLifxBatchCreator
{
    public class NameToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new SolidColorBrush(Colors.White);

            int test;
            var color = value.ToString();
            if (string.IsNullOrEmpty(color))
                return new SolidColorBrush(Colors.White);

            if (int.TryParse(color, out test))
                return new SolidColorBrush(Kelvin.Kelvins[color]);

            var colorBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString(color));

            return colorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SolidColorBrush).Color;
        }
    }
}