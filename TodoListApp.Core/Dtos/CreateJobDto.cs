namespace TodoListApp.Core.Dtos;

public class CreateJobDto
{
    public int EmployeerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0;
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
}
