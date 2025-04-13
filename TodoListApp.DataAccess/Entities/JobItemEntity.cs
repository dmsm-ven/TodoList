namespace TodoListApp.DataAccess.Entities;

public class JobItemEntity
{
    public int id { get; set; }
    public int employeer_id { get; set; }
    public string title { get; set; }
    public string? description { get; set; }
    public string? website { get; set; }
    public bool is_completed { get; set; }
    public bool is_payed { get; set; }
    public decimal price { get; set; }
    public DateTimeOffset start_date { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? end_date { get; set; }
}
