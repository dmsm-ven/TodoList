using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models.ShoppingItem;

internal class ShoppingItemViewModel : ViewModelBase
{
    public int Id { get; set; }
    string name;
    public string Name
    {
        get => name;
        set => Set(ref name, value);
    }
    bool isPurchased;
    public bool IsPurchased
    {
        get => isPurchased;
        set
        {
            if(Set(ref isPurchased, value))
            {
                DatePurchased = value ? (DateTime?)DateTime.Now : null;
                RaisePropertyChanged(nameof(StateIcon));
            }
        }
    }
    public DateTime DateAdded { get; set; }
    public DateTime? DatePurchased { get; set; }
    public ICommand ToggleStatusCommand { get; }

    public ShoppingItemViewModel()
    {
        ToggleStatusCommand = new LambdaCommand(e => IsPurchased = !IsPurchased);
    }

    public PackIconFontAwesomeKind StateIcon
    {
        get => IsPurchased ? PackIconFontAwesomeKind.EyeSlashRegular : PackIconFontAwesomeKind.CheckSquareRegular;
    }

}
