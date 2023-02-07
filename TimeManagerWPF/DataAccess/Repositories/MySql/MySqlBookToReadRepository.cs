using System.Collections.Generic;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess.Repositories;

public class MySqlBookToReadRepository : IBookToReadRepository
{
    private readonly IDapperDatabaseAccess database;

    public MySqlBookToReadRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }
    public int AddOrUpdate(BookToReadEntity item)
    {
        string sql = @"INSERT INTO book_to_read (Id, Name, Author, Image, DateEnded) 
                        VALUES(@Id, @Name, @Author, @Image, @DateEnded) 
                        ON DUPLICATE KEY UPDATE 
                            Name = @Name,
                            Author = @Author,
                            Image = @Image,
                            DateEnded = @DateEnded";


        database.Execute(sql, item);

        int id = item.id != 0 ?
            item.id :
            database.GetSingle<int>("SELECT MAX(Id) FROM book_to_read");

        return id;
    }

    public void Delete(int id)
    {
        string sql = "DELETE FROM book_to_read WHERE Id = @id";
        database.Execute(sql, new { id });
    }

    public IEnumerable<BookToReadEntity> GetAll()
    {
        string sql = "SELECT * FROM book_to_read";

        var items = database.GetList<BookToReadEntity>(sql);
        return items;
    }
}