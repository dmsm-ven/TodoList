using TodoListApp.Core.Entities;

namespace TodoListApp.Core.Repositories.Interfaces;

public interface IEmployeerRepository
{
    Task<List<EmployeerEntity>> GetAllEmployeer();
    EmployeerEntity GetEmployeer(int id);
    void DeleteEmployeer(int id);
    int AddOrUpdateEmployeer(EmployeerEntity entity);
}
