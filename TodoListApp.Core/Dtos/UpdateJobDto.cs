namespace TodoListApp.Core.Dtos;

public class UpdateJobDto
{
    public int Id { get; set; }
    public int EmployeerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0;
    public bool IsCompleted { get; set; }
    public bool IsPayed { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
}
