namespace TodoListApp.Core.Entities;

public class LogEntryEntity
{
    public int id { get; set; }
    public DateTimeOffset date_time { get; set; }
    public string message { get; set; }
}
