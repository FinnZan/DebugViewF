using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReferenceViewer
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;

            return b ? Color.FromRgb(0xEE, 0xEE, 0xEE) : Color.FromRgb(0xFF, 0xEE, 0xEE);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}