namespace TodoListApp.Core.Repositories.Interfaces;

/// <summary>Один SQL-запрос с параметрами (для выполнения в составе транзакции)</summary>
public record SqlStatement(string Sql, object? Parameters = null);

public interface IDapperDatabaseAccess
{
    List<T> GetList<T>(string sql, object parameters = null);
    Task<List<T>> GetListAsync<T>(string sql, object parameters = null);
    void Execute(string sql, object parameters = null);
    Task ExecuteAsync(string sql, object parameters = null);

    /// <summary>Выполняет все запросы в одной транзакции: либо все применяются, либо ни один</summary>
    Task ExecuteInTransactionAsync(IEnumerable<SqlStatement> statements);

    T GetSingle<T>(string sql, object parameters = null);
    Task<T> GetSingleAsync<T>(string sql, object parameters = null);
}
