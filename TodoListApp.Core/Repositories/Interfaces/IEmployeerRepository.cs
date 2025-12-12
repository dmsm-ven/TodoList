using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;

namespace TodoListApp.Core.Repositories.Interfaces;

public interface IEmployeerRepository
{
    Task<List<EmployeerEntity>> GetAllEmployeer();
    Task<EmployeerEntity> GetEmployeer(int id);
    Task DeleteEmployeer(int id);
    Task<int> AddEmployeer(EmployeerEntity entity);
    Task<List<EmployeerPaymentEntity>> GetAllPaymentsForEmployeer(int employeer_id);
    Task AddPayment(EmployeerPaymentPayload payment);
}
