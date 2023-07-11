using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories.Interfaces;

public interface IShoppingItemsRepository
{
    IEnumerable<ShoppingItemEntity> GetAll();
    int AddOrUpdate(ShoppingItemEntity item);
    void Delete(int id);
    int AddCategory(string name);
}
