using AutoMapper;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoList.WPF.DataAccess;
using TodoList.WPF.Models.ShoppingItem;

namespace TodoList.WPF.ViewModels;

internal class ShoppingListViewModel : ViewModelBase
{
    private readonly IShoppingItemsRepository repository;
    private readonly IMapper mapper;

    public ObservableCollection<ShoppingItemViewModel> Items { get; }

    public ICommand AddNewItemCommand { get; }

    public ShoppingListViewModel()
    {
        AddNewItemCommand = new LambdaCommand(AddNewItem);
        Items = new ObservableCollection<ShoppingItemViewModel>();
    }

    private void AddNewItem(object obj)
    {
        var item = new ShoppingItemViewModel()
        {
            DateAdded = DateTime.Now,
            Name = "Новый товар"
        };
        item.Id = repository.AddOrUpdate(mapper.Map<ShoppingItemEntity>(item));
        Items.Insert(0, item);
    }

    public ShoppingListViewModel(IShoppingItemsRepository repository, IMapper mapper) : this()
    {
        this.repository = repository;
        this.mapper = mapper;
    }
}
