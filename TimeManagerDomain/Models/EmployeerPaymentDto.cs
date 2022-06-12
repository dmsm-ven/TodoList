using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Domain;

public class EmployeerPaymentDto
{
    [Key]
    public int Id { get; set; }
    public int EmployeerId { get; set; }
    public DateTime TransferArrivalDate { get; set; }
    public int Amount { get; set; }
}
