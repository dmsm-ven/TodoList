using System.Collections.Generic;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.Core.Repositories.Postgres.Repositories;

public class PostgresAppLogger : IAppLogger
{
    private readonly IDapperDatabaseAccess dapper;

    public PostgresAppLogger(IDapperDatabaseAccess dapper)
    {
        this.dapper = dapper;
    }

    public List<LogEntryEntity> GetLastRows(int takeCount)
    {
        var items = dapper.GetList<LogEntryEntity>("SELECT * FROM log_entry ORDER BY id DESC LIMIT @takeCount"
            , new { takeCount });
        return items;
    }

    public void WriteLog(string message)
    {
        dapper.Execute("INSERT INTO log_entry (message) VALUES (@message)", new { message });
    }
}



