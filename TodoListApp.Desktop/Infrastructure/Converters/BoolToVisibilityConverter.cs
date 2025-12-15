using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TodoListApp.Desktop.Infrastructure.Converters;


public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Visibility result = Visibility.Visible;

        if (value is bool b)
        {
            result = b ? Visibility.Visible : Visibility.Collapsed;
        }
        else if (value is string s)
        {
            result = (!string.IsNullOrWhiteSpace(s)) ? Visibility.Visible : Visibility.Collapsed;
        }
        else
        {
            result = value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        // inverse
        if (parameter is string strParam)
        {
            if (strParam == "inverse" || strParam == "reverse" || strParam == "invert")
            {
                result = (result == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            }
            if (strParam == "hidden")
            {
                result = (result == Visibility.Collapsed) ? Visibility.Hidden : result;
            }
        }


        return result;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
