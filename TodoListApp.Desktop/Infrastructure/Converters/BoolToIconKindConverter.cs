using System;
using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace TodoListApp.Desktop.Infrastructure.Converters;

public class BoolToIconKindConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? SymbolRegular.Checkmark24 : SymbolRegular.Subtract24;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
