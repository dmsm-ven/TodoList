using AutoMapper;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TodoList.WPF.Models.Budget;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoList.WPF.ViewModels;

public class BudgetViewModel : ViewModelBase
{
    private readonly IBudgetRepository repository;
    private readonly IMapper mapper;

    public ICommand ShowNewItemPanelCommand { get; }
    public ICommand AddNewItemCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand LoadedCommand { get; }
    public ICommand SetNewItemTypeCommand { get; }

    private DateTime? filterStartDate;
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

    private DateTime? filterEndDate;
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

    private BudgetItemModel newBudgetItem;
    public BudgetItemModel NewBudgetItem
    {
        get => newBudgetItem;
        set => Set(ref newBudgetItem, value);
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
        ShowNewItemPanelCommand = new LambdaCommand(e => NewBudgetItem = new BudgetItemModel(), e => NewBudgetItem == null);
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


        FilteredSource = CollectionViewSource.GetDefaultView(BudgetLines);
        FilteredSource.SortDescriptions.Add(new SortDescription(nameof(BudgetItemModel.Created), ListSortDirection.Descending));
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
        bool isInvalid = NewBudgetItem == null ||
            NewBudgetItem.Category == null ||
            string.IsNullOrWhiteSpace(NewBudgetItem.Title) ||
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
        NewBudgetItem = null;
    }

    private async Task<bool> AddCategoryOrCancel()
    {
        /*if (AvailableCategories.FirstOrDefault(i => i.Name.Trim().Equals(NewCategoryName.Trim(), StringComparison.OrdinalIgnoreCase)) == null)
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
        */
        return true;
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
            category.IsCheckedChanged += () =>
            {
                if (NewBudgetItem != null)
                {
                    NewBudgetItem.Category = category.IsChecked ? category : null;
                }
            };
        }
    }
}


