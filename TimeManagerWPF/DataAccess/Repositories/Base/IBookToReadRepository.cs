using System.Collections.Generic;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess.Repositories;

public interface IBookToReadRepository
{
    IEnumerable<BookToReadEntity> GetAll();
    int AddOrUpdate(BookToReadEntity item);
    void Delete(int id);
}
