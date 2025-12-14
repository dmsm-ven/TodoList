namespace TodoListApp.Core.Entities;

public class LogEntryEntity
{
    public int id { get; set; }
    public DateTime date_time { get; set; }
    public string message { get; set; } = string.Empty;
    public string client_ip { get; set; } = string.Empty;
    public string path { get; set; } = string.Empty;
}
