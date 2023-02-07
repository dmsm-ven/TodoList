using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace TodoList.WPF.DataAccess;

public interface IDapperDatabaseAccess
{
    List<T> GetList<T>(string sql, object parameters = null);
    T GetSingle<T>(string sql, object parameters = null);
    void Execute(string sql, object parameters = null);
}
