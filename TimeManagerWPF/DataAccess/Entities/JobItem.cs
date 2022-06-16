using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoList.DataAccess;

public class JobItemEntity
{
    [Key]
    public int Id { get; set; }
    public int EmployeerId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPayed { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime? EndDate { get; set; }
}
