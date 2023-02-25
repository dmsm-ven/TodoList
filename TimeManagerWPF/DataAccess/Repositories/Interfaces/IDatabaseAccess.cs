using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoList.WPF.DataAccess;

public interface IDapperDatabaseAccess
{
    List<T> GetList<T>(string sql, object parameters = null);
    Task<List<T>> GetListAsync<T>(string sql, object parameters = null);
    void Execute(string sql, object parameters = null);
    Task ExecuteAsync(string sql, object parameters = null);

    T GetSingle<T>(string sql, object parameters = null);
    Task<T> GetSingleAsync<T>(string sql, object parameters = null);
}
