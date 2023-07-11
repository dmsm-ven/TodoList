using System.Collections.Generic;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories.Interfaces;

public interface IEmployeerRepository
{
    IEnumerable<EmployeerEntity> GetAllEmployeer();
    EmployeerEntity GetEmployeer(int id);
    void DeleteEmployeer(int id);
    int AddOrUpdateEmployeer(EmployeerEntity entity);
}
