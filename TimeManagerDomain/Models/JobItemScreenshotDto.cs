using System.ComponentModel.DataAnnotations;

namespace TodoList.Domain;

public class JobItemScreenshotDto
{
    [Key]public int Id { get; set; }
    public int JobItemId { get; set; }
    public string FileName { get; set; }
    public int SortOrder { get; set; }
}