using AutoMapper;
using LiveCharts;
using LiveCharts.Wpf;
using MahApps.Metro.IconPacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TodoList.WPF.DataAccess.Entities;
using TodoList.WPF.DataAccess.Repositories.Interfaces;
using TodoList.WPF.Infrastructure.Extensions;
using TodoList.WPF.Models.Budget;

namespace TodoList.WPF.ViewModels;

public class BudgetViewModel : ViewModelBase
{
    private readonly IBudgetRepository repository;
    private readonly IMapper mapper;

    public ICommand AddNewItemCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand LoadedCommand { get; }
    public ICommand SetNewItemTypeCommand { get; }

    DateTime? filterStartDate;
    public DateTime? FilterStartDate
    {
        get => filterStartDate;
        set
        {
            if (Set(ref filterStartDate, value))
            {
                FilteredSource.Refresh();
                RaisePropertyChanged(nameof(SeriesCollection));
            }
        }
    }

    DateTime? filterEndDate;
    public DateTime? FilterEndDate
    {
        get => filterEndDate;
        set
        {
            if (Set(ref filterEndDate, value))
            {
                FilteredSource.Refresh();
                RaisePropertyChanged(nameof(SeriesCollection));
            }
        }
    }

    public BudgetItemType[] AvailableTypes { get; } = Enum.GetValues<BudgetItemType>();

    BudgetItemModel newBudgetItem = new();
    public BudgetItemModel NewBudgetItem
    {
        get => newBudgetItem;
        set => Set(ref newBudgetItem, value);
    }

    string newCategoryName;
    public string NewCategoryName
    {
        get => newCategoryName;
        set => Set(ref newCategoryName, value);
    }

    public ObservableCollection<BudgetCategoryModel> AvailableCategories { get; }

    public ObservableCollection<BudgetItemModel> BudgetLines { get; }

    public ColorsCollection ColorsCollection { get; }

    public SeriesCollection SeriesCollection
    {
        get
        {
            var filteredSource = FilteredSource.OfType<BudgetItemModel>();
            var incomeSeriesData = filteredSource.Where(i => i.BudgetType == BudgetItemType.Income).ToArray();
            var outcomeSeriesData = filteredSource.Where(i => i.BudgetType == BudgetItemType.Outcome).ToArray();

            var sc = new SeriesCollection();
            sc.Add(new LineSeries()
            {
                Title = "Расход",
                Values = new ChartValues<decimal>(incomeSeriesData.Select(i => i.Amount))
            });
            sc.Add(new LineSeries()
            {
                Title = "Доход",
                Values = new ChartValues<decimal>(outcomeSeriesData.Select(i => i.Amount))
            });

            return sc;
        }
    }

    public ICollectionView FilteredSource { get; }

    public BudgetViewModel()
    {
        LoadedCommand = new LambdaCommand(async (e) => await Loaded());
        AddNewItemCommand = new LambdaCommand(async (e) => await AddNewItem(), CanAddNewBudgetItem);
        ClearFilterCommand = new LambdaCommand(ClearFilter, CanClearFilter);
        SetNewItemTypeCommand = new LambdaCommand(SetNewItemType);

        AvailableCategories = new ObservableCollection<BudgetCategoryModel>();
        BudgetLines = new ObservableCollection<BudgetItemModel>();
        ColorsCollection = new ColorsCollection()
    {
        System.Windows.Media.Color.FromRgb(255, 0, 0),
        System.Windows.Media.Color.FromRgb(124, 252, 0),
    };

        InsertTestData();

        FilteredSource = CollectionViewSource.GetDefaultView(BudgetLines);
        FilteredSource.Filter = (o) =>
        {
            if (FilterStartDate == null && FilterEndDate == null)
            {
                return true;
            }

            var item = o as BudgetItemModel;

            if (FilterStartDate.HasValue && item.Created < FilterStartDate.Value)
            {
                return false;
            }
            if (FilterEndDate.HasValue && item.Created > FilterEndDate.Value)
            {
                return false;
            }

            return true;
        };


    }

    private void SetNewItemType(object obj)
    {
        if (Enum.TryParse<BudgetItemType>(obj.ToString(), out var t))
        {
            NewBudgetItem.BudgetType = t;
        }
    }

    public BudgetViewModel(IBudgetRepository repository, IMapper mapper) : this()
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    private bool CanAddNewBudgetItem(object arg)
    {
        bool isInvalid = string.IsNullOrWhiteSpace(NewBudgetItem.Title) ||
            string.IsNullOrWhiteSpace(NewCategoryName) ||
            NewBudgetItem.BudgetType == BudgetItemType.None ||
            NewBudgetItem.Amount == 0;

        return !isInvalid;
    }

    private async Task AddNewItem()
    {
        if (!(await AddCategoryOrCancel()))
        {
            MessageBox.Show("Действие отменено", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        BudgetLines.Add(NewBudgetItem);
        var entity = mapper.Map<BudgetItemEntity>(NewBudgetItem);
        await repository.AddBudgetItem(entity);

        FilteredSource.Refresh();
        NewBudgetItem = new BudgetItemModel();
    }

    private async Task<bool> AddCategoryOrCancel()
    {
        if (AvailableCategories.FirstOrDefault(i => i.Name.Trim().Equals(NewCategoryName.Trim(), StringComparison.OrdinalIgnoreCase)) == null)
        {
            var dialog = MessageBox.Show($"Категории [{NewCategoryName}] не существует. Создать ее ?", "Подтверждение", MessageBoxButton.YesNoCancel, MessageBoxImage.Question); ;
            if (dialog == MessageBoxResult.Yes)
            {
                var cat = new BudgetCategoryEntity() { name = NewCategoryName, icon = "QuestionSolid" };
                await repository.AddCategory(cat);

                cat.id = (AvailableCategories.OrderByDescending(c => c.Id)?.FirstOrDefault()?.Id + 1) ?? 1;
                var model = mapper.Map<BudgetCategoryModel>(cat);
                AvailableCategories.Add(model);
                newBudgetItem.Category = model;

                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private void InsertTestData()
    {
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

    private bool CanClearFilter(object arg)
    {
        return new[] { FilterStartDate, FilterEndDate }.Any(v => v != null);
    }

    private void ClearFilter(object obj)
    {
        filterStartDate = null;
        filterEndDate = null;
        FilteredSource.Refresh();
        RaisePropertyChanged(nameof(SeriesCollection));
    }

    private async Task Loaded()
    {
        BudgetLines.Clear();
        AvailableCategories.Clear();
        FilteredSource.Refresh();

        var budgetItems = await repository.GetBudgetItems();
        var budgetItemModels = mapper.Map<IEnumerable<BudgetItemModel>>(budgetItems);
        foreach (var budgetItem in budgetItemModels)
        {
            BudgetLines.Add(budgetItem);
        }

        var budgetCategories = await repository.GetBudgeCategories();
        var budgetCategoryModels = mapper.Map<IEnumerable<BudgetCategoryModel>>(budgetCategories);
        foreach (var category in budgetCategoryModels)
        {
            AvailableCategories.Add(category);
        }
    }
}


