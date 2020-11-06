using System;
using System.Globalization;
using System.Windows.Data;

namespace FileManipulator.UI
{
    public class EqualToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value?.ToString() == parameter?.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool b && b ? parameter : null;
    }
}
