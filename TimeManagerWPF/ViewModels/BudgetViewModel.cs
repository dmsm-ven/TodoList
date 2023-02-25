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
using System.Windows.Data;
using System.Windows.Input;
using TodoList.WPF.DataAccess.Repositories.Interfaces;
using TodoList.WPF.Infrastructure.Extensions;
using TodoList.WPF.Models.Budget;

namespace TodoList.WPF.ViewModels;

public class BudgetViewModel : ViewModelBase
{
    private readonly IBudgetRepository repository;
    private readonly IMapper mapper;

    public ICommand ClearFilterCommand { get; }
    public ICommand LoadedCommand { get; }

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
        ClearFilterCommand = new LambdaCommand(ClearFilter, CanClearFilter);
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

    public BudgetViewModel(IBudgetRepository repository, IMapper mapper) : this()
    {
        this.repository = repository;
        this.mapper = mapper;
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
        FilteredSource.Refresh();

        var items = await repository.GetBudgetItems();
        var itemsModel = mapper.Map<List<BudgetItemModel>>(items);

        foreach (var item in itemsModel)
        {
            BudgetLines.Add(item);
        }
    }
}


