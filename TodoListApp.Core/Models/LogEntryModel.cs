using Humanizer;
using System;
using System.Collections.Generic;
using System.Text;
using TodoListApp.Core.Entities;

namespace TodoListApp.Core.Models;

public class LogEntryModel
{
    public DateTimeOffset Created { get; set; }
    public string ClientIp { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Elapsed => (DateTimeOffset.UtcNow - Created).Humanize();

    public static LogEntryModel FromEntity(LogEntryEntity entity)
    {
        return new LogEntryModel
        {
            Created = TimeZoneInfo.ConvertTimeFromUtc(entity.date_time, TimeZoneInfo.Local),
            Message = entity.message,
            Path = entity.path,
            ClientIp = entity.client_ip
        };
    }
}
