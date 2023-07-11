using System.Collections.Generic;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoListApp.DataAccess.Repositories.Postgres.Repositories;

public class PostgresEmployeerPaymentRepository : IEmployeerPaymentRepository
{
    private readonly IDapperDatabaseAccess database;

    public PostgresEmployeerPaymentRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public IEnumerable<EmployeerPaymentEntity> GetAllPaymentsForEmployeer(int employeer_id)
    {
        var sql = "SELECT * FROM employeer_payment WHERE employeer_id = @employeer_id";
        var items = database.GetList<EmployeerPaymentEntity>(sql, new { employeer_id });
        return items;
    }

    public void AddPayment(EmployeerPaymentEntity payment)
    {
        string sql = @"INSERT INTO employeer_payment (employeer_id, amount, transfer_arrival_date) VALUES
                                                     (@employeer_id, @amount, @transfer_arrival_date)";

        database.Execute(sql, payment);
    }
}