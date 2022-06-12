using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TodoList.WPF.Infrastructure.Converters;

public class IsActiveMonthForegroundBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Brushes.Yellow : Brushes.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
