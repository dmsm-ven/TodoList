using MahApps.Metro.IconPacks;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TodoList.WPF.Infrastructure.Converters;

public class BoolToIconKindConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? PackIconFontAwesomeKind.CheckSolid : PackIconFontAwesomeKind.TimesSolid;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
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
        if (parameter != null)
        {
            result = (result == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        return result;
       
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
