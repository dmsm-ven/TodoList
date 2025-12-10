using Humanizer;
using System;
using System.Collections.Generic;
using System.Text;
using TodoListApp.Core.Entities;

namespace TodoListApp.Core.Models;

public class LogEntryModel
{
    public DateTimeOffset Created { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Elapsed => (DateTimeOffset.UtcNow - Created).Humanize();

    public static LogEntryModel FromEntity(LogEntryEntity entity)
    {
        return new LogEntryModel
        {
            Created = entity.date_time,
            Message = entity.message
        };
    }
}
