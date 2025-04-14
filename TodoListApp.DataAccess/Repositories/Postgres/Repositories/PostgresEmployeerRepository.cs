using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoListApp.DataAccess.Repositories.Postgres.Repositories;

public class PostgresEmployeerRepository : IEmployeerRepository
{
    private readonly IDapperDatabaseAccess database;

    public PostgresEmployeerRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public int AddOrUpdateEmployeer(EmployeerEntity entity)
    {
        string sql = @"INSERT INTO employeer (id, name) VALUES(@id, @name) 
                       ON CONFLICT(id) DO UPDATE 
                       SET name = @name";

        database.Execute(sql, entity);

        int id = entity.id != 0 ?
            entity.id :
            database.GetSingle<int>("SELECT MAX(id) FROM employeer");

        return id;
    }

    public void DeleteEmployeer(int id)
    {
        database.Execute("DELETE FROM employeer WHERE id = @id", new { id });
    }

    public async Task<List<EmployeerEntity>> GetAllEmployeer()
    {
        var items = await database.GetListAsync<EmployeerEntity>("SELECT * FROM employeer");
        return items;
    }

    public EmployeerEntity GetEmployeer(int id)
    {
        var employeer = database.GetSingle<EmployeerEntity>("SELECT * FROM employeer WHERE id = @id", new { id });
        return employeer;
    }
}