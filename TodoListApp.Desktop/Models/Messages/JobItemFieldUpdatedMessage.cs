using CommunityToolkit.Mvvm.Messaging.Messages;
using TodoListApp.Desktop.ViewModels;

namespace TodoListApp.Desktop.Models.Messages;

public class JobItemFieldUpdatedMessage : ValueChangedMessage<JobItemViewModel>
{
    public string FieldName { get; }
    public string FieldValue { get; }

    public JobItemFieldUpdatedMessage(JobItemViewModel item, string fieldName, string fieldValue) : base(item)
    {
        FieldName = fieldName;
        FieldValue = fieldValue;
    }
}