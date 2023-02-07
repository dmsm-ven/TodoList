using Dapper;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.WPF.DataAccess;

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

    public List<T> GetList<T>(string sql, object parameters = null)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);

        connection.Open();

        var list = connection.Query<T>(sql, parameters).ToList();

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
