using System;

namespace TodoList.WPF.Models.Budget;

public class BudgetItemModel
{
    public int Id { get; init; }
    public DateTimeOffset Created { get; init; }
    public BudgetItemType BudgetType { get; init; }
    public BudgetItemTransactionCurrency Currency { get; init; }
    public decimal Amount { get; init; }
    public decimal? AmountRUBEquivalent { get; init; }
    public string Title { get; init; } = String.Empty;
    public string Description { get; init; } = String.Empty;

    public BudgetCategoryModel Category { get; init; }
}
