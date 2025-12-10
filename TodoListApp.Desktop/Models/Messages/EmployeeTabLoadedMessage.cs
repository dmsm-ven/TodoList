using CommunityToolkit.Mvvm.Messaging.Messages;
using TodoList.WPF.Models.TodoList;

namespace TodoList.WPF.Models.Messages;
public class EmployeeTabLoadedMessage : ValueChangedMessage<EmployeerTabViewModel>
{
    public EmployeeTabLoadedMessage(EmployeerTabViewModel value) : base(value)
    {

    }
}