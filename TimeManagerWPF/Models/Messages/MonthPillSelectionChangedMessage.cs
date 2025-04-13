using CommunityToolkit.Mvvm.Messaging.Messages;
using TodoList.WPF.Models.TodoList;

namespace TodoList.WPF.Models.Messages;

public class MonthPillSelectionChangedMessage : ValueChangedMessage<EmployeerTabViewModel>
{
    public MonthPillSelectionChangedMessage(EmployeerTabViewModel value) : base(value)
    {
    }
}
