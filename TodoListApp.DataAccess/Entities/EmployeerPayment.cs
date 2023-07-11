using System;
using System.ComponentModel.DataAnnotations;

namespace TodoListApp.DataAccess.Entities;

public class EmployeerPaymentEntity
{
    public int id { get; set; }
    public int employeer_id { get; set; }
    public DateTime transfer_arrival_date { get; set; }
    public decimal amount { get; set; }
}
