using System.Collections.Generic;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories.Interfaces;

public interface IBookToReadRepository
{
    IEnumerable<BookToReadEntity> GetAll();
    int AddOrUpdate(BookToReadEntity item);
    void Delete(int id);
}
