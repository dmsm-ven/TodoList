using System;
using System.Globalization;
using System.Windows.Data;

namespace TodoList.WPF.Infrastructure.Converters;

public class EqualsStringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && parameter != null && value.ToString().Equals(parameter.ToString()))
        {
            return true;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
