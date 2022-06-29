using MahApps.Metro.IconPacks;
using System;
using System.Globalization;
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
