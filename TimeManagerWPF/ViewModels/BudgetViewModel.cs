using MahApps.Metro.IconPacks;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TodoList.WPF.Infrastructure.Extensions;
using TodoList.WPF.Models.Budget;

namespace TodoList.WPF.ViewModels;

public class BudgetViewModel : ViewModelBase
{
    public ICommand LoadedCommand { get; }

    public ObservableCollection<BudgetItemModel> BudgetLines { get; } = new();

    public BudgetViewModel()
    {
        LoadedCommand = new LambdaCommand(Loaded);

        int i = 0;
        foreach (var number in Enumerable.Range(0, 15))
        {
            var income = new BudgetItemModel()
            {
                Amount = i * 250 + 3000,
                AmountRUBEquivalent = null,
                BudgetType = BudgetItemType.Income,
                Category = new BudgetCategoryModel()
                {
                    Id = 1,
                    Name = "Работа",
                    Icon = PackIconFontAwesomeKind.SuitcaseSolid
                },
                Created = DateTimeOffset.Now.AddDays(-i),
                Currency = BudgetItemTransactionCurrency.RUB,
                Id = ++i,
                Title = "Аванс етк",
                Description = "Аванс от етк за период с х по х"
            };
            BudgetLines.Add(income);

            var outcome = new BudgetItemModel()
            {
                Amount = i * 100 + 1500,
                AmountRUBEquivalent = null,
                BudgetType = BudgetItemType.Outcome,
                Category = new BudgetCategoryModel()
                {
                    Id = 2,
                    Name = "Продукты",
                    Icon = PackIconFontAwesomeKind.ShoppingBasketSolid
                },
                Created = DateTimeOffset.Now.AddDays(-i),
                Currency = BudgetItemTransactionCurrency.RUB,
                Id = ++i,
                Title = "Аванс етк",
                Description = "Аванс от етк за период с х по х"
            };
            BudgetLines.Add(outcome);
        }
    }

    private void Loaded(object obj)
    {
        //BudgetLines.Clear();
    }
}


