using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TodoListApp.Desktop.Infrastructure.Converters;

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool inverse = (parameter?.ToString().Contains("inverse")) ?? false;

        Visibility visibility = Visibility.Hidden;

        if ((value != default && !inverse) || (value == default && inverse))
        {
            visibility = Visibility.Visible;
        }

        return visibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
