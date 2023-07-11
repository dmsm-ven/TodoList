using System;

namespace TodoListApp.DataAccess.Entities;

public class BudgetItemEntity
{
    public int id { get; set; }
    public DateTime created { get; set; } = DateTime.UtcNow;
    public int budget_type { get; set; }
    public string currency { get; set; }
    public decimal amount { get; set; }
    public decimal? amount_rub_equivalent { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; }

    public int category_id { get; set; }
    public BudgetCategoryEntity category { get; set; }
}
