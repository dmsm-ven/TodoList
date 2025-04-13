using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories.Interfaces;

public interface IEmployeerPaymentRepository
{
    Task<List<EmployeerPaymentEntity>> GetAllPaymentsForEmployeer(int employeer_id);
    void AddPayment(EmployeerPaymentEntity payment);
}
