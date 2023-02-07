using System.Collections.Generic;
using TodoList.DataAccess;

namespace TodoList.WPF.DataAccess;

public class PostgresEmployeerPaymentRepository : IEmployeerPaymentRepository
{
    private readonly IDapperDatabaseAccess database;

    public PostgresEmployeerPaymentRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public IEnumerable<EmployeerPaymentEntity> GetAllPaymentsForEmployeer(int employeer_id)
    {
        var items = database.GetList<EmployeerPaymentEntity>("SELECT * FROM employeer_payment WHERE EmployeerId = @employeer_id", new { employeer_id });
        return items;
    }

    public void AddPayment(EmployeerPaymentEntity payment)
    {
        string sql = @"INSERT INTO employeer_payment (EmployeerId, Amount, TransferArrivalDate) VALUES
                                                     (@EmployeerId, @Amount, @TransferArrivalDate)";

        database.Execute(sql, payment);
    }
}