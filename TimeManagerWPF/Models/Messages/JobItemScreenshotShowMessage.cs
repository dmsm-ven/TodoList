using TodoList.WPF.Models.TodoList;

namespace TodoList.WPF.Models.Messages;

public record JobItemScreenshotShowMessage(EmployeerTabViewModel parentView, int jobItemId);
