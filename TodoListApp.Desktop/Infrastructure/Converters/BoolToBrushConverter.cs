using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TodoListApp.Desktop.Infrastructure.Converters;
public class BoolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!string.IsNullOrWhiteSpace(parameter.ToString()))
        {
            string[] colors = parameter.ToString().Split(';');
            if (colors.Length == 2)
            {
                Color colorTrue = (Color)ColorConverter.ConvertFromString(colors[0]);
                Color colorFalse = (Color)ColorConverter.ConvertFromString(colors[1]);
                var result = (bool?)value;
                if (result.HasValue)
                {
                    return new SolidColorBrush(result.Value ? colorTrue : colorFalse);
                }
            }
        }

        return Brushes.Fuchsia;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}