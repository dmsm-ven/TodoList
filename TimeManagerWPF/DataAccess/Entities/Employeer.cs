using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList.DataAccess;

public class EmployeerEntity
{
    [Key]
    public int Id { get; set; }  
    [Required]
    public string Name { get; set; } = String.Empty;
    public DateTime Created { get; set; }
    public IEnumerable<EmployeerPaymentEntity> Payments { get; set; }
    public IEnumerable<JobItemEntity> Jobs { get; set; }
}
