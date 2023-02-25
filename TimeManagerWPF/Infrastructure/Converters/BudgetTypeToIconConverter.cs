using MahApps.Metro.IconPacks;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TodoList.WPF.Models.Budget;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Infrastructure.Converters;

public class BudgetTypeToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BudgetItemType i && i != BudgetItemType.None)
        {
            return i == BudgetItemType.Income ? PackIconFontAwesomeKind.PlusSolid : PackIconFontAwesomeKind.MinusSolid;
        }
        return PackIconFontAwesomeKind.QuestionCircleSolid;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BudgetTypeToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BudgetItemType i && i != BudgetItemType.None)
        {
            return i == BudgetItemType.Income ? Brushes.LawnGreen : Brushes.OrangeRed;
        }
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
