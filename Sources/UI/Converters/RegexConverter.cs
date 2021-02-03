using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileManipulator.UI
{
    [ValueConversion(typeof(Regex), typeof(string))]
    public class RegexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrEmpty(s))
            {
                try
                {
                    return new Regex(s);
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }
            else
                return null;
        }
    }
}
