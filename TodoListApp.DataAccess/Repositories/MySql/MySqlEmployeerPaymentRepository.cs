using System.Collections.Generic;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoListApp.DataAccess.Repositories.MySql;

public class MySqlEmployeerPaymentRepository : IEmployeerPaymentRepository
{
    private readonly IDapperDatabaseAccess database;

    public MySqlEmployeerPaymentRepository(IDapperDatabaseAccess database)
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