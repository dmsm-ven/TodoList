using System.Collections.Generic;
using TodoList.DataAccess;

namespace TodoList.WPF.DataAccess;

public interface IEmployeerRepository
{
    IEnumerable<EmployeerEntity> GetAllEmployeer();
    EmployeerEntity GetEmployeer(int id);
    void DeleteEmployeer(int id);
    int AddOrUpdateEmployeer(EmployeerEntity entity);
}
