using System.Collections.Generic;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoListApp.DataAccess.Repositories.MySql;

public class MySqlShoppingItemsRepository : IShoppingItemsRepository
{
    private readonly IDapperDatabaseAccess database;

    public MySqlShoppingItemsRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public int AddCategory(string name)
    {
        database.Execute("INSERT INTO shopping_item_category (Name) VALUES (@name)", new { name });

        int id = database.GetSingle<int>("SELECT MAX(Id) FROM shopping_item_category");

        return id;
    }

    public int AddOrUpdate(ShoppingItemEntity item)
    {
        string sql = @"INSERT INTO shopping_item (Id, Name, IsPurchased, DatePurchased, CategoryId) 
                        VALUES(@Id, @Name, @IsPurchased, @DatePurchased, @CategoryId) 
                        ON DUPLICATE KEY UPDATE 
                            Name = @Name,
                            IsPurchased = @IsPurchased,
                            DatePurchased = @DatePurchased,
                            CategoryId = @CategoryId";


        database.Execute(sql, item);

        int id = item.id != 0 ?
            item.id :
            database.GetSingle<int>("SELECT MAX(Id) FROM shopping_item");

        return id;
    }

    public int AddOrUpdate(ShoppingItemCategoryEntity category, ShoppingItemEntity item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        string sql = "DELETE FROM shopping_item WHERE Id = @id";
        database.Execute(sql, new { id });
    }

    public IEnumerable<ShoppingItemEntity> GetAll()
    {
        string sql = @"SELECT sa.*, sac.Name as CategoryName, sac.Id as CategoryId
                       FROM shopping_item sa
                       LEFT JOIN shopping_item_category sac ON (sa.CategoryId = sac.Id)";

        var items = database.GetList<ShoppingItemEntity>(sql);
        return items;
    }
}
