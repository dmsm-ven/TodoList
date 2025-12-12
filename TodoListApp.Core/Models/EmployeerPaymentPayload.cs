using System;
using System.Collections.Generic;
using System.Text;

namespace TodoListApp.Core.Models;

public class EmployeerPaymentPayload
{
    public int employeer_id { get; set; }
    public DateTimeOffset transfer_arrival_date { get; set; }
    public decimal amount { get; set; }
}
