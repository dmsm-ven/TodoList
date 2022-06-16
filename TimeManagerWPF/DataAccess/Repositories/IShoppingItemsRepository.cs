using System.Collections.Generic;
using TodoList.DataAccess;
using TodoList.WPF.DataAccess;

namespace TodoList.WPF.DataAccess;

public interface IShoppingItemsRepository
{
    IEnumerable<ShoppingItemEntity> GetAll();
    int AddOrUpdate(ShoppingItemEntity item);
    void Delete(int id);

}

public class ShoppingItemsRepository : IShoppingItemsRepository
{
    private readonly IDapperDatabaseAccess database;

    public ShoppingItemsRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }
    public int AddOrUpdate(ShoppingItemEntity item)
    {
        string sql = @"INSERT INTO shopping_item (Id, Name, IsPurchased, DatePurchased) 
                        VALUES(@Id, @Name, @IsPurchased, @DatePurchased) 
                        ON DUPLICATE KEY UPDATE 
                            Name = @Name,
                            IsPurchased = @IsPurchased,
                            DatePurchased = @DatePurchased";
    

        database.Execute(sql, item);

        int id = item.Id != 0 ?
            item.Id :
            database.GetSingle<int>("SELECT MAX(Id) FROM shopping_item");

        return id;
    }

    public void Delete(int id)
    {
        string sql = "DELETE FROM shopping_item WHERE Id = @id";
        database.Execute(sql, new { id });
    }

    public IEnumerable<ShoppingItemEntity> GetAll()
    {
        string sql = "SELECT * FROM shopping_item";

        var items = database.GetList<ShoppingItemEntity>(sql);
        return items;
    }
}
