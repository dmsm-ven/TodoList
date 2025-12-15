using CommunityToolkit.Mvvm.Messaging.Messages;
using TodoListApp.Desktop.Models.TodoList;

namespace TodoListApp.Desktop.Models.Messages;

public class EmployeeTabLoadedMessage : ValueChangedMessage<EmployeerTabViewModel>
{
    public EmployeeTabLoadedMessage(EmployeerTabViewModel value) : base(value)
    {

    }
}