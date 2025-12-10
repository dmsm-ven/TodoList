using System;
using System.Globalization;
using System.Windows.Data;

namespace TodoListApp.Desktop.Infrastructure.Converters;

public class ReferenceEqualsToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && parameter != null)
        {
            return ReferenceEquals(value, parameter);
        }
        return false;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
