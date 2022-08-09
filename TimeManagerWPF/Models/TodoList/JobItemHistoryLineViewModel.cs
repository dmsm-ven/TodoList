using System;

namespace TodoList.WPF.Models;

public record JobItemHistoryLineViewModel
{
    public DateTime DateTime { get; init; }
    public string ChangedPropertyName { get; init; }
    public string NewValue { get; init; }
}
