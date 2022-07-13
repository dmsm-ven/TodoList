using AutoMapper;
using TodoList.WPF.DataAccess;
using TodoList.WPF.Models.ShoppingItem;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Automapper.Profiles;

public class ShoppingItemProfile : Profile
{
    public ShoppingItemProfile()
    {
        CreateMap<ShoppingItemViewModel, ShoppingItemEntity>();
        CreateMap<ShoppingItemEntity, ShoppingItemViewModel>();

        CreateMap<ShoppingItemCategoryEntity, ShoppingItemCategory>();
        CreateMap<ShoppingItemCategory, ShoppingItemCategoryEntity>();
    }
}
