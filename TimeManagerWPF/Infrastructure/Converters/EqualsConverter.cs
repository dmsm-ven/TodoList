using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TodoList.WPF.Infrastructure.Converters;

public class ActiveMenuToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ViewModelType input  = (ViewModelType)value;
        ViewModelType activeNow = (ViewModelType)parameter;
        if(input == activeNow)
        {
            return Brushes.Yellow;
        }
        return Brushes.White;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
