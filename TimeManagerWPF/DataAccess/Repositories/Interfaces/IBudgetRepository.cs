using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess.Repositories.Interfaces;

public interface IBudgetRepository
{
    Task<BudgetItemEntity[]> GetBudgetItems();
    Task AddBudgetItem(BudgetItemEntity item);

    Task<BudgetCategoryEntity[]> GetBudgeCategories();
    Task AddCategory(BudgetCategoryEntity category);
}
