namespace TodoListApp.Core.Models;

public record JobItemHistoryLineModel(DateTimeOffset DateTime, string ChangedPropertyName, string NewValue);