using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.Core.Repositories.Postgres.Repositories;

public class PostgresEmployeerRepository : IEmployeerRepository
{
    private readonly IDapperDatabaseAccess database;

    public PostgresEmployeerRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public async Task<int> AddEmployeer(EmployeerEntity entity)
    {
        string sql = @"INSERT INTO employeer (id, name) VALUES(@id, @name) 
                       ON CONFLICT(id) DO UPDATE 
                       SET name = @name";

        await database.ExecuteAsync(sql, entity);

        int id = entity.id != 0 ?
            entity.id :
            database.GetSingle<int>("SELECT MAX(id) FROM employeer");

        return id;
    }

    public async Task DeleteEmployeer(int id)
    {
        await database.ExecuteAsync("DELETE FROM employeer WHERE id = @id", new { id });
    }

    public async Task<List<EmployeerEntity>> GetAllEmployeer()
    {
        var items = await database.GetListAsync<EmployeerEntity>("SELECT * FROM employeer");
        return items;
    }

    public async Task<EmployeerEntity> GetEmployeer(int id)
    {
        var employeer = await database.GetSingleAsync<EmployeerEntity>("SELECT * FROM employeer WHERE id = @id", new { id });
        return employeer;
    }

    public async Task<List<EmployeerPaymentEntity>> GetAllPaymentsForEmployeer(int employeer_id)
    {
        var sql = "SELECT * FROM employeer_payment WHERE employeer_id = @employeer_id";
        var items = await database.GetListAsync<EmployeerPaymentEntity>(sql, new { employeer_id });
        return items;
    }

    public async Task AddPayment(EmployeerPaymentPayload payment)
    {
        string sql = @"INSERT INTO employeer_payment (employeer_id, amount, transfer_arrival_date) VALUES
                                                     (@employeer_id, @amount, @transfer_arrival_date)";

        await database.ExecuteAsync(sql, payment);
    }
}