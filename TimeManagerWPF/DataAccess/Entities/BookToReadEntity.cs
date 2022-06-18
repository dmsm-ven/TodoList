using System;

namespace TodoList.WPF.DataAccess.Entities;

public class BookToReadEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Image { get; set; }
    public bool IsAlreadyReaded { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateEnded { get; set; }
}
