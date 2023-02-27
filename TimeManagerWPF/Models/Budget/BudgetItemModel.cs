using System;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models.Budget;

public class BudgetItemModel : ViewModelBase
{
    public int Id { get; init; }
    public DateTime Created { get; init; } = DateTime.UtcNow;

    BudgetItemType budgetType;
    public BudgetItemType BudgetType
    {
        get => budgetType;
        set => Set(ref budgetType, value);
    }

    BudgetItemTransactionCurrency currency;
    public BudgetItemTransactionCurrency Currency
    {
        get => currency;
        set => Set(ref currency, value);
    }

    decimal amount;
    public decimal Amount
    {
        get => amount;
        set => Set(ref amount, value);
    }

    decimal? amountRUBEquivalent;
    public decimal? AmountRUBEquivalent
    {
        get => amountRUBEquivalent;
        set => Set(ref amountRUBEquivalent, value);
    }

    string title;
    public string Title
    {
        get => title;
        set => Set(ref title, value);
    }

    string description;
    public string Description
    {
        get => description;
        set => Set(ref description, value);
    }

    BudgetCategoryModel category;
    public BudgetCategoryModel Category
    {
        get => category;
        set => Set(ref category, value);
    }
}
