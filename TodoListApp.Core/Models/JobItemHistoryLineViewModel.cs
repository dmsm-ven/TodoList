using Humanizer;

namespace TodoListApp.Core.Models;

public class JobItemHistoryLineModel
{
    public required DateTime LocalTime { get; init; }
    public required string ChangedPropertyName { get; init; }
    public required string NewValue { get; init; }

    public string Elapsed => (DateTime.Now - LocalTime).Humanize();
}