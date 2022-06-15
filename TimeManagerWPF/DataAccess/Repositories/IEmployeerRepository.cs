using System.Collections.Generic;
using TodoList.DataAccess;

namespace TodoList.WPF.DataAccess;

public interface IEmployeerRepository
{
    IEnumerable<EmployeerEntity> GetAllEmployeer();
    EmployeerEntity GetEmployeer(int id);
    void DeleteEmployeer(int id);
    int AddOrUpdateEmployeer(EmployeerEntity entity);
}

public class EmployeerRepository : IEmployeerRepository
{
    private readonly IDapperDatabaseAccess database;

    public EmployeerRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public int AddOrUpdateEmployeer(EmployeerEntity entity)
    {
        string sql = @"INSERT INTO employeer (Id, EmployeerName) 
                        VALUES(Id, EmployeerName) 
                        ON DUPLICATE KEY UPDATE 
                            EmployeerName = @EmployeerName";

        database.Execute(sql, entity);

        int id = entity.Id != 0 ?
            entity.Id :
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