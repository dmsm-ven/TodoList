using System.Collections.Generic;
using System.Threading.Tasks;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories.Interfaces;

public interface IBudgetRepository
{
    Task<BudgetItemEntity[]> GetBudgetItems();
    Task AddBudgetItem(BudgetItemEntity item);

    Task<BudgetCategoryEntity[]> GetBudgeCategories();
    Task AddCategory(BudgetCategoryEntity category);
}
