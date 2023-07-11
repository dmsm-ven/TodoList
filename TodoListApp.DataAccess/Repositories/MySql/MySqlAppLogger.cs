using System.Collections.Generic;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoListApp.DataAccess.Repositories.MySql;

public class MySqlAppLogger : IAppLogger
{
    private readonly IDapperDatabaseAccess dapper;

    public MySqlAppLogger(IDapperDatabaseAccess dapper)
    {
        this.dapper = dapper;
    }

    public List<LogEntryEntity> GetLastRows(int takeCount)
    {
        var items = dapper.GetList<LogEntryEntity>("SELECT * FROM log_entry ORDER BY Id DESC LIMIT @takeCount"
            , new { takeCount });
        return items;
    }

    public void WriteLog(string message)
    {
        dapper.Execute("INSERT INTO log_entry (message) VALUES (@message)", new { message });
    }
}
