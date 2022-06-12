using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Domain;

public class JobItemDto
{
    [Key]
    public int Id { get; set; }
    public int EmployeerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Website { get; set; }
    public bool IsCompleted { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }   
    public DateTime? EndDate { get; set; }

    public virtual ICollection<JobItemScreenshotDto> Screenshots { get; set; }
}
