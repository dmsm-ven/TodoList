using System.Collections.Generic;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories.Interfaces;

public interface IEmployeerPaymentRepository
{
    IEnumerable<EmployeerPaymentEntity> GetAllPaymentsForEmployeer(int employeer_id);
    void AddPayment(EmployeerPaymentEntity payment);
}
