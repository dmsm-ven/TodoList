using TodoListApp.Core.Entities;

namespace TodoListApp.Core.Repositories.Interfaces;

public interface IEmployeerPaymentRepository
{
    Task<List<EmployeerPaymentEntity>> GetAllPaymentsForEmployeer(int employeer_id);
    void AddPayment(EmployeerPaymentEntity payment);
}
