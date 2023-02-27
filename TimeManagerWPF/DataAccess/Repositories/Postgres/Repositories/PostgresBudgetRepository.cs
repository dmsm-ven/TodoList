using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.WPF.DataAccess.Entities;
using TodoList.WPF.DataAccess.Repositories.Interfaces;

namespace TodoList.WPF.DataAccess.Repositories.Postgres;

public class PostgresBudgetRepository : IBudgetRepository
{
    private readonly IDapperDatabaseAccess database;

    public PostgresBudgetRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public async Task<BudgetItemEntity[]> GetBudgetItems()
    {
        var list = await database.GetListAsync<BudgetItemEntity>("SELECT * FROM budget_item");

        return list.ToArray();
    }

    public async Task AddBudgetItem(BudgetItemEntity item)
    {

        var sql = @"INSERT INTO budget_item (created, budget_type, amount, currency, amount_rub_equivalent, title, description, category_id) VALUES
                            (@created, @budget_type, @amount, @currency, @amount_rub_equivalent, @title, @description, @category_id)";

        await database.ExecuteAsync(sql, item);
    }

    public async Task AddCategory(BudgetCategoryEntity category)
    {
        var sql = @"INSERT INTO budget_category (icon, name) VALUES (@icon, @name)";

        await database.ExecuteAsync(sql, category);
    }

    public async Task<BudgetCategoryEntity[]> GetBudgeCategories()
    {
        var list = await database.GetListAsync<BudgetCategoryEntity>("SELECT * FROM budget_category");

        return list.ToArray();
    }


}
