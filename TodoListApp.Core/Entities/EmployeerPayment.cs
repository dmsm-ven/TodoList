namespace TodoListApp.Core.Entities;

public class EmployeerPaymentEntity
{
    public int id { get; set; }
    public int employeer_id { get; set; }
    public DateTimeOffset transfer_arrival_date { get; set; }
    public decimal amount { get; set; }
}
