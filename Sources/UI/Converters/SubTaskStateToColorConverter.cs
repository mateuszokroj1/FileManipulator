using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace FileManipulator.UI
{
    [ValueConversion(typeof(SubTaskState), typeof(SolidColorBrush))]
    internal class SubTaskStateToColorConverter : IValueConverter
    {
        public static readonly Dictionary<SubTaskState, SolidColorBrush> Values = new Dictionary<SubTaskState, SolidColorBrush>
        {
            { SubTaskState.Ready, new SolidColorBrush(Color.FromRgb(170,170,170)) },
            { SubTaskState.Pending, new SolidColorBrush(Color.FromRgb(41, 106, 194)) },
            { SubTaskState.Working, new SolidColorBrush(Color.FromRgb(238, 194, 0)) },
            { SubTaskState.Done, new SolidColorBrush(Color.FromRgb(90, 220, 0)) }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SubTaskState state)
            {
                return Values[state];
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return Values.Where(pair => pair.Value.Equals(brush)).FirstOrDefault();
            }
            else
                return null;
        }
    }
}
