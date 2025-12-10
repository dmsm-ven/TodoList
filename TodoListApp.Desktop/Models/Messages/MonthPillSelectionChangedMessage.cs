using CommunityToolkit.Mvvm.Messaging.Messages;
using TodoList.WPF.Models.TodoList;

namespace TodoList.WPF.Models.Messages;

public class MonthPillSelectionChangedMessage : ValueChangedMessage<MonthPillViewModel>
{
    public MonthPillSelectionChangedMessage(MonthPillViewModel value) : base(value)
    {
    }
}
