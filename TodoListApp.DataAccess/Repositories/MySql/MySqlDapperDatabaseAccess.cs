//sing MySql.Data.MySqlClient;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoListApp.DataAccess.Repositories.MySql;

public class MySqlDapperDatabaseAccess : IDapperDatabaseAccess
{
    private readonly string connectionString;

    public MySqlDapperDatabaseAccess(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void Execute(string sql, object parameters = null)
    {
        throw new NotImplementedException();
        //using IDbConnection connection =new MySqlConnection(connectionString);

        //connection.Open();

        //connection.Execute(sql, parameters);
    }

    public Task ExecuteAsync(string sql, object parameters = null)
    {
        throw new NotImplementedException();
    }

    public List<T> GetList<T>(string sql, object parameters = null)
    {
        throw new NotImplementedException();
        //using IDbConnection connection = new MySqlConnection(connectionString);

        //connection.Open();

        //var list = connection.Query<T>(sql, parameters).ToList();

        //return list;
    }

    public Task<List<T>> GetListAsync<T>(string sql, object parameters = null)
    {
        throw new NotImplementedException();
    }

    public T GetSingle<T>(string sql, object parameters = null)
    {
        throw new NotImplementedException();
        //using IDbConnection connection = new MySqlConnection(connectionString);

        //connection.Open();

        //var item = connection.QuerySingleOrDefault<T>(sql, parameters);

        //return item;
    }

    public Task<T> GetSingleAsync<T>(string sql, object parameters = null)
    {
        throw new NotImplementedException();
    }
}
