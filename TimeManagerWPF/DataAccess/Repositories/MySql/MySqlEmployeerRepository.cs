using System.Collections.Generic;
using TodoList.DataAccess;

namespace TodoList.WPF.DataAccess;

public class MySqlEmployeerRepository : IEmployeerRepository
{
    private readonly IDapperDatabaseAccess database;

    public MySqlEmployeerRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public int AddOrUpdateEmployeer(EmployeerEntity entity)
    {
        string sql = @"INSERT INTO employeer (Id, Name) 
                        VALUES(@Id, @Name) 
                        ON DUPLICATE KEY UPDATE 
                            Name = @Name";

        database.Execute(sql, entity);

        int id = entity.id != 0 ?
            entity.id :
            database.GetSingle<int>("SELECT MAX(Id) FROM employeer");

        return id;
    }

    public void DeleteEmployeer(int id)
    {
        database.Execute("DELETE FROM employeer WHERE Id = @id", new { id });
    }

    public IEnumerable<EmployeerEntity> GetAllEmployeer()
    {
        var items = database.GetList<EmployeerEntity>("SELECT * FROM employeer");
        return items;
    }

    public EmployeerEntity GetEmployeer(int id)
    {
        var employeer = database.GetSingle<EmployeerEntity>("SELECT * FROM employeer WHERE Id = @id", new { id });
        return employeer;
    }
}