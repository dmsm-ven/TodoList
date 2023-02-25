using AutoMapper;
using System;
using TodoList.WPF.DataAccess.Entities;
using TodoList.WPF.Infrastructure.Extensions;
using TodoList.WPF.Models.Budget;

namespace TodoList.WPF.Infrastructure.Automapper.Profiles;

public class BudgetItemProfile : Profile
{
    public BudgetItemProfile()
    {
        CreateMap<BudgetItemEntity, BudgetItemModel>()
            .ForMember(x => x.Id, x => x.MapFrom(o => o.id))
            .ForMember(x => x.Amount, x => x.MapFrom(o => o.amount))
            .ForMember(x => x.AmountRUBEquivalent, x => x.MapFrom(o => o.amount_rub_equivalent))
            .ForMember(x => x.BudgetType, x => x.MapFrom(o => Enum.Parse<BudgetItemType>(o.budget_type)))
            .ForMember(x => x.Title, x => x.MapFrom(o => o.title))
            .ForMember(x => x.Description, x => x.MapFrom(o => o.description))
            .ForMember(x => x.Currency, x => x.MapFrom(o => Enum.Parse<BudgetItemTransactionCurrency>(o.currency)))
            .ForMember(x => x.Category, x => x.MapFrom(o => new BudgetCategoryModel()
            {
                Id = o.category_id,
                Name = o.category.name,
                Icon = o.category.icon.ToFontAwesomeIcon()
            }));
    }
}
