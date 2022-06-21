using System;
using System.Globalization;
using System.Windows.Data;

namespace TodoList.WPF.Infrastructure.Converters;

public class ReadedBookOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? 0.2 : 1.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
