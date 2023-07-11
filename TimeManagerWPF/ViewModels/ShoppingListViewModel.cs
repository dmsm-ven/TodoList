using AutoMapper;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TodoList.WPF.Models.ShoppingItem;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoList.WPF.ViewModels;

internal class ShoppingListViewModel : ViewModelBase
{
    private readonly IShoppingItemsRepository repository;
    private readonly IMapper mapper;

    public ObservableCollection<ShoppingItemViewModel> Items { get; }

    public Dictionary<string, List<ShoppingItemViewModel>> FilteredItems
    {
        get
        {
            var source = Items
                .Where(item => ShowHidden ? true : item.IsPurchased == false)
                .OrderByDescending(item => item.DateAdded)
                .GroupBy(item => item.CategoryName ?? "Другое")
                .ToDictionary(g => g.Key, g => g.ToList());

            return source;
        }
    }

    public List<ShoppingItemCategory> UniqueCategories
    {
        get => Items
            .GroupBy(item => item.CategoryId)
            .Select(g => new ShoppingItemCategory() { Id = g.Key, Name = g.FirstOrDefault()?.CategoryName ?? "NULL" })
            .ToList();
    }

    private ShoppingItemCategory selectedNewItemCategory;
    public ShoppingItemCategory SelectedNewItemCategory
    {
        get => selectedNewItemCategory;
        set => Set(ref selectedNewItemCategory, value);
    }

    private bool showHidden;
    public bool ShowHidden
    {
        get => showHidden;
        set
        {
            if (Set(ref showHidden, value))
            {
                RaisePropertyChanged(nameof(ShowHiddenIcon));
                RaisePropertyChanged(nameof(ShowHiddenTitle));
                RaisePropertyChanged(nameof(FilteredItems));
            }
        }
    }

    private string newItemText;
    public string NewItemText
    {
        get => newItemText;
        set => Set(ref newItemText, value);
    }
    public PackIconFontAwesomeKind ShowHiddenIcon
    {
        get => ShowHidden ? PackIconFontAwesomeKind.EyeSlashSolid : PackIconFontAwesomeKind.EyeSolid;
    }
    public string ShowHiddenTitle
    {
        get => ShowHidden ? "Скрыть завершенные" : "Отобразить завершенные";
    }

    public ICommand ShowHiddenToggleCommand { get; }
    public ICommand AddNewItemCommand { get; }
    public ICommand LoadedCommand { get; }

    public ShoppingListViewModel(IShoppingItemsRepository repository, IMapper mapper) : this()
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    public ShoppingListViewModel()
    {
        ShowHiddenToggleCommand = new LambdaCommand(e => ShowHidden = !ShowHidden);
        AddNewItemCommand = new LambdaCommand(AddNewItem, e => !string.IsNullOrWhiteSpace(NewItemText));
        LoadedCommand = new LambdaCommand(Loaded);
        Items = new ObservableCollection<ShoppingItemViewModel>();
        AddDesignTimeItems();
        Items.CollectionChanged += (o, e) => RaisePropertyChanged(nameof(FilteredItems));
    }

    private void AddDesignTimeItems()
    {
        var categories = new[] { "Дом", "Одежда", "Другое" };
        foreach (var cat in categories)
        {
            for (int i = 0; i < 10; i++)
            {
                Items.Add(new ShoppingItemViewModel()
                {
                    Name = $"{cat} item {i + 1}",
                    DateAdded = DateTime.Now,
                    IsPurchased = new Random().Next(2) == 0 ? true : false,
                    CategoryName = cat
                });
            }
        }
        RaisePropertyChanged(nameof(FilteredItems));
    }

    private void Loaded(object obj)
    {
        Items?.ToList().ForEach(item => item.PropertyChanged -= Item_PropertyChanged);

        Items?.Clear();

        repository.GetAll().ToList()
            .ForEach(item =>
            {
                var vmItem = mapper.Map<ShoppingItemViewModel>(item);
                Items.Add(vmItem);
                vmItem.PropertyChanged += Item_PropertyChanged;
            });

        RaisePropertyChanged(nameof(UniqueCategories));
    }

    private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ShoppingItemViewModel.IsPurchased))
        {
            RaisePropertyChanged(nameof(FilteredItems));
            RaisePropertyChanged(nameof(UniqueCategories));

            var item = mapper.Map<ShoppingItemEntity>(sender as ShoppingItemViewModel);

            repository.AddOrUpdate(item);
        }
    }

    private void AddNewItem(object obj)
    {
        var item = new ShoppingItemViewModel()
        {
            DateAdded = DateTime.Now,
            Name = NewItemText,
            CategoryId = SelectedNewItemCategory?.Id
        };

        if ((selectedNewItemCategory?.Id ?? 0) == 0)
        {
            item.CategoryId = repository.AddCategory(SelectedNewItemCategory?.Name);
            UniqueCategories.Insert(0, new ShoppingItemCategory() { Id = item.CategoryId, Name = item.CategoryName });
            SelectedNewItemCategory = UniqueCategories.First();
        }

        var itemEntity = mapper.Map<ShoppingItemEntity>(item);

        item.Id = repository.AddOrUpdate(itemEntity);
        item.PropertyChanged += Item_PropertyChanged;
        Items.Insert(0, item);

        NewItemText = string.Empty;
        SelectedNewItemCategory = null;

    }
}
