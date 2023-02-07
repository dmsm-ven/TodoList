using System.Collections.Generic;
using TodoList.DataAccess;
using TodoList.WPF.DataAccess;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess;

public interface IShoppingItemsRepository
{
    IEnumerable<ShoppingItemEntity> GetAll();
    int AddOrUpdate(ShoppingItemEntity item);
    void Delete(int id);
    int AddCategory(string name);
}
