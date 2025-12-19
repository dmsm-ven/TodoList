using CommunityToolkit.Mvvm.Messaging.Messages;
using TodoListApp.Desktop.ViewModels;

namespace TodoListApp.Desktop.Models.Messages;

public class JobItemFieldUpdatedMessage : ValueChangedMessage<JobItemViewModel>
{
    public JobItemFieldUpdatedMessage(JobItemViewModel item) : base(item) { }
}