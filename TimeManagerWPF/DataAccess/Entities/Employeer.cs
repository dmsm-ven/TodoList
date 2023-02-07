using System;

namespace TodoList.DataAccess;

public class EmployeerEntity
{
    public int id { get; set; }
    public string name { get; set; } = String.Empty;
    public string icon { get; set; } = String.Empty;
    public string email { get; set; } = String.Empty;
    public string phone_number { get; set; } = String.Empty;
    public DateTime created { get; set; } = DateTime.Now;
}
