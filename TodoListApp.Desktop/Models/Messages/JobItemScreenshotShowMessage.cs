using TodoListApp.Desktop.Models.TodoList;

namespace TodoListApp.Desktop.Models.Messages;

public record JobItemScreenshotShowMessage(EmployeerTabViewModel parentView, int jobItemId);
