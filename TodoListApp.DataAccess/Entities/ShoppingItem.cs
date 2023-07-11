using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoListApp.DataAccess.Entities;

public class ShoppingItemEntity
{
    public int id { get; set; }
    public int? category_id { get; set; }
    public string name { get; set; }
    public string? category_name { get; set; }
    public bool is_purchased { get; set; }
    public DateTime date_added { get; set; }
    public DateTime? date_purchased { get; set; }
}


