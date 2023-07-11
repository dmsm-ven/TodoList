using System;

namespace TodoListApp.DataAccess.Entities;

public class BookToReadEntity
{
    public int id { get; set; }
    public string name { get; set; }
    public string author { get; set; }
    public string image { get; set; }
    public DateTime date_added { get; set; }
    public DateTime? date_ended { get; set; }
}
