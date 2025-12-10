using CommunityToolkit.Mvvm.Messaging.Messages;
using TodoListApp.Desktop.Models.TodoList;

namespace TodoListApp.Desktop.Models.Messages;

public class MonthPillSelectionChangedMessage : ValueChangedMessage<MonthPillViewModel>
{
    public MonthPillSelectionChangedMessage(MonthPillViewModel value) : base(value)
    {
    }
}
