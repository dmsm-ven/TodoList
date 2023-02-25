using System.Collections.Generic;

namespace TodoList.WPF.DataAccess;

public class PostgresShoppingItemsRepository : IShoppingItemsRepository
{
    private readonly IDapperDatabaseAccess database;

    public PostgresShoppingItemsRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public int AddCategory(string name)
    {
        database.Execute("INSERT INTO shopping_item_category (name) VALUES (@name)", new { name });

        int id = database.GetSingle<int>("SELECT MAX(id) FROM shopping_item_category");

        return id;
    }

    public int AddOrUpdate(ShoppingItemEntity item)
    {
        string sql = @"INSERT INTO shopping_item (id, name, is_purchased, date_purchased, category_id) 
                        VALUES(@id, @name, @is_purchased, @date_purchased, @category_id) 
                        ON CONFLICT(id) DO UPDATE
                        SET name = @name,
                            is_purchased = @is_purchased,
                            date_purchased = @date_purchased,
                            category_id = @category_id";


        database.Execute(sql, item);

        int id = item.id != 0 ?
            item.id :
            database.GetSingle<int>("SELECT MAX(id) FROM shopping_item");

        return id;
    }

    public int AddOrUpdate(ShoppingItemCategoryEntity category, ShoppingItemEntity item)
    {
        throw new System.NotImplementedException();
    }

    public void Delete(int id)
    {
        string sql = "DELETE FROM shopping_item WHERE id = @id";
        database.Execute(sql, new { id });
    }

    public IEnumerable<ShoppingItemEntity> GetAll()
    {
        string sql = @"SELECT sa.*, sac.name as CategoryName, sac.id as CategoryId
                       FROM shopping_item sa
                       LEFT JOIN shopping_item_category sac ON (sa.category_id = sac.id)";

        var items = database.GetList<ShoppingItemEntity>(sql);
        return items;
    }
}
