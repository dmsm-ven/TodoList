using Dapper;
using Npgsql;
using System.Data;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.Core.Repositories.Postgres.Base;

public class PostgresDapperDatabaseAccess : IDapperDatabaseAccess
{
    private readonly string connectionString;

    public PostgresDapperDatabaseAccess(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void Execute(string sql, object parameters = null)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);

        connection.Open();

        connection.Execute(sql, parameters);
    }

    public async Task ExecuteAsync(string sql, object parameters = null)
    {
        using var connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        await connection.ExecuteAsync(sql, parameters);
    }

    public List<T> GetList<T>(string sql, object parameters = null)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);

        connection.Open();

        var list = connection.Query<T>(sql, parameters).ToList();

        return list;
    }

    public async Task<List<T>> GetListAsync<T>(string sql, object parameters = null)
    {
        using var connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        var list = (await connection.QueryAsync<T>(sql, parameters)).ToList();

        return list;
    }

    public T GetSingle<T>(string sql, object parameters = null)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);

        connection.Open();

        var item = connection.QuerySingleOrDefault<T>(sql, parameters);

        return item;
    }

    public async Task<T> GetSingleAsync<T>(string sql, object parameters = null)
    {
        using var connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        var item = await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);

        return item;
    }
}
