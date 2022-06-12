using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList.Domain;

public class EmployeerDto
{
    [Key]
    public int Id { get; set; }  
    [Required]
    public string Name { get; set; } = String.Empty;
    public DateTime Created { get; set; }
    public virtual ICollection<EmployeerPaymentDto> Payments { get; set; }
    public virtual ICollection<JobItemDto> Jobs { get; set; }
}
