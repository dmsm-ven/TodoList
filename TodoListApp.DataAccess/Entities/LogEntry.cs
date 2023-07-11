using System;

namespace TodoListApp.DataAccess.Entities;

public class LogEntryEntity
{
    public int id { get; set; }
    public DateTime date_time { get; set; }
    public string message { get; set; }
}
