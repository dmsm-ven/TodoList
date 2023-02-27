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
        CreateMap<BudgetCategoryEntity, BudgetCategoryModel>()
            .ForMember(x => x.Id, x => x.MapFrom(o => o.id))
            .ForMember(x => x.Name, x => x.MapFrom(o => o.name))
            .ForMember(x => x.Icon, x => x.MapFrom(o => o.icon.ToFontAwesomeIcon()));

        CreateMap<BudgetItemEntity, BudgetItemModel>()
            .ForMember(x => x.Id, x => x.MapFrom(o => o.id))
            .ForMember(x => x.Created, x => x.MapFrom(o => o.created))
            .ForMember(x => x.Amount, x => x.MapFrom(o => o.amount))
            .ForMember(x => x.AmountRUBEquivalent, x => x.MapFrom(o => o.amount_rub_equivalent))
            .ForMember(x => x.BudgetType, x => x.MapFrom(o => (BudgetItemType)o.budget_type))
            .ForMember(x => x.Title, x => x.MapFrom(o => o.title))
            .ForMember(x => x.Description, x => x.MapFrom(o => o.description))
            .ForMember(x => x.Currency, x => x.MapFrom(o => Enum.Parse<BudgetItemTransactionCurrency>(o.currency)))
            .ForMember(x => x.Category, x => x.MapFrom(o => new BudgetCategoryModel()
            {
                Id = o.category_id,
                Name = o.category.name,
                Icon = o.category.icon.ToFontAwesomeIcon()
            }));

        CreateMap<BudgetItemModel, BudgetItemEntity>()
            .ForMember(x => x.id, x => x.MapFrom(o => o.Id))
            .ForMember(x => x.amount, x => x.MapFrom(o => o.Amount))
            .ForMember(x => x.created, x => x.MapFrom(o => o.Created))
            .ForMember(x => x.amount_rub_equivalent, x => x.MapFrom(o => o.Amount))
            .ForMember(x => x.budget_type, x => x.MapFrom(o => (int)o.BudgetType))
            .ForMember(x => x.title, x => x.MapFrom(o => o.Title))
            .ForMember(x => x.description, x => x.MapFrom(o => o.Description))
            .ForMember(x => x.currency, x => x.MapFrom(o => o.Currency.ToString()))
            .ForMember(x => x.category_id, x => x.MapFrom(o => o.Category.Id))
            .ForMember(x => x.category, x => x.Ignore());
    }
}
