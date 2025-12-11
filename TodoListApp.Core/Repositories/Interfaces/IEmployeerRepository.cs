using TodoListApp.Core.Entities;

namespace TodoListApp.Core.Repositories.Interfaces;

public interface IEmployeerRepository
{
    Task<List<EmployeerEntity>> GetAllEmployeer();
    Task<EmployeerEntity> GetEmployeer(int id);
    Task DeleteEmployeer(int id);
    Task<int> AddOrUpdateEmployeer(EmployeerEntity entity);
    Task<List<EmployeerPaymentEntity>> GetAllPaymentsForEmployeer(int employeer_id);
    Task AddPayment(EmployeerPaymentEntity payment);
}
