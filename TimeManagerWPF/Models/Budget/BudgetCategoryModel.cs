using MahApps.Metro.IconPacks;
using System;
using System.ComponentModel;
using System.Windows.Input;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models.Budget;

public class BudgetCategoryModel : ViewModelBase
{
    public event Action IsCheckedChanged;

    public int Id { get; init; }
    public string Name { get; init; }
    public PackIconFontAwesomeKind Icon { get; init; }

    bool isChecked;
    public bool IsChecked
    {
        get => isChecked;
        set
        {
            if (Set(ref isChecked, value))
            {
                IsCheckedChanged?.Invoke();
            }
        }
    }

}
