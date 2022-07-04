using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess.Repositories;

public interface IAppLogger
{
    void WriteLog(string message);
    List<LogEntryEntity> GetLastRows(int takeCount);
}

public class AppLogger : IAppLogger
{
    private readonly IDapperDatabaseAccess dapper;

    public AppLogger(IDapperDatabaseAccess dapper)
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

