using System.Collections.Generic;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess.Repositories;

public class PostgresBookToReadRepository : IBookToReadRepository
{
    private readonly IDapperDatabaseAccess database;

    public PostgresBookToReadRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }
    public int AddOrUpdate(BookToReadEntity item)
    {
        string sql = @"INSERT INTO book_to_read (id, name, author, image, date_ended) 
                        VALUES(@id, @name, @author, @image, @date_ended) 
                        ON DUPLICATE KEY UPDATE 
                            name = @name,
                            author = @author,
                            image = @image,
                            date_ended = @date_ended";


        database.Execute(sql, item);

        int id = item.id != 0 ?
            item.id :
            database.GetSingle<int>("SELECT MAX(id) FROM book_to_read");

        return id;
    }

    public void Delete(int id)
    {
        string sql = "DELETE FROM book_to_read WHERE id = @id";
        database.Execute(sql, new { id });
    }

    public IEnumerable<BookToReadEntity> GetAll()
    {
        string sql = "SELECT * FROM book_to_read";

        var items = database.GetList<BookToReadEntity>(sql);
        return items;
    }
}