namespace TodoListApp.DataAccess.Entities;

public class EmployeerEntity
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string icon { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string phone_number { get; set; } = string.Empty;
    public DateTimeOffset created { get; set; } = DateTimeOffset.Now;
}
