using System.Collections.Generic;
using TodoList.DataAccess;

namespace TodoList.WPF.DataAccess;

public interface IEmployeerPaymentRepository
{
    IEnumerable<EmployeerPaymentEntity> GetAllPaymentsForEmployeer(int employeer_id);
    void AddPayment(EmployeerPaymentEntity payment);
}
