using MahApps.Metro.IconPacks;
using System;
using System.Globalization;
using System.Windows.Data;

namespace TodoListApp.Desktop.Infrastructure.Converters;

public class BoolToIconKindConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? PackIconFontAwesomeKind.CheckSolid :
            PackIconFontAwesomeKind.MinusSolid;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
