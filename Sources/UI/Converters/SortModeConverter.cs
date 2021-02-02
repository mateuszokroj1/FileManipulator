using System;
using System.Globalization;
using System.Windows.Data;

using FileManipulator.Models.Manipulator;

namespace FileManipulator.UI
{
    [ValueConversion(typeof(SortMode), typeof(string))]
    public class SortModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SortMode mode)
            {
                return mode == SortMode.Ascending ? "Rosnąco" : "Malejąco";
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as string) == "Rosnąco")
                return SortMode.Ascending;
            else if ((value as string) == "Malejąco")
                return SortMode.Descending;
            else
                return null;
        }
    }
}
