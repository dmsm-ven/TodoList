using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories.Interfaces;

public interface IEmployeerRepository
{
    Task<List<EmployeerEntity>> GetAllEmployeer();
    EmployeerEntity GetEmployeer(int id);
    void DeleteEmployeer(int id);
    int AddOrUpdateEmployeer(EmployeerEntity entity);
}
