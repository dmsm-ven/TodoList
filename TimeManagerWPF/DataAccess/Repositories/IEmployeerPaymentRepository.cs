using System.Collections.Generic;
using TodoList.DataAccess;

namespace TodoList.WPF.DataAccess;

public interface IEmployeerPaymentRepository
{
    IEnumerable<EmployeerPaymentEntity> GetAllPaymentsForEmployeer(int employeer_id);
    void AddPayment(EmployeerPaymentEntity payment);
}

public class EmployeerPaymentRepository : IEmployeerPaymentRepository
{
    private readonly IDapperDatabaseAccess database;

    public EmployeerPaymentRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public IEnumerable<EmployeerPaymentEntity> GetAllPaymentsForEmployeer(int employeer_id)
    {
        var items = database.GetList<EmployeerPaymentEntity>("SELECT * FROM employeer_payment");
        return items;
    }

    public void AddPayment(EmployeerPaymentEntity payment)
    {
        string sql = @"INSERT INTO employeer_payment (Id, EmployeerId, Amount, TransferArrivalDate"; 

        database.Execute(sql, payment);
    }
}