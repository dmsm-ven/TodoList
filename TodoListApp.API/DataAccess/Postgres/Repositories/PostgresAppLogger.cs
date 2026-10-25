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

    public async Task<List<LogEntryEntity>> GetLastRows(int takeCount)
    {
        var items = await dapper.GetListAsync<LogEntryEntity>("SELECT * FROM log_entry ORDER BY id DESC LIMIT @takeCount"
            , new { takeCount });
        return items;
    }

    public async Task WriteLog(string clientIp, string path, string message)
    {
        await dapper.ExecuteAsync("INSERT INTO log_entry (date_time, client_ip, path, message) VALUES (@dt, @client_ip, @path, @message)", new
        {
            client_ip = clientIp,
            path,
            message,
            // UTC, не зависит от часового пояса контейнера. Kind=Unspecified, т.к. Npgsql не пишет Kind=Utc в "timestamp without time zone"
            dt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
        });
    }
}



