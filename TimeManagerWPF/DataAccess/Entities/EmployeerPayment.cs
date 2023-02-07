using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.DataAccess;

public class EmployeerPaymentEntity
{
    public int id { get; set; }
    public int employeer_id { get; set; }
    public DateTime transfer_arrival_date { get; set; }
    public decimal Amount { get; set; }
}
