using TodoListApp.Core.Entities;

namespace TodoListApp.Core.Repositories.Interfaces;

public interface IAppLogger
{
    Task WriteLog(string userIp, string path, string message);
    Task<List<LogEntryEntity>> GetLastRows(int take_count);
}



