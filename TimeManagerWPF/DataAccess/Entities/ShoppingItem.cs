using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.WPF.DataAccess;

public class ShoppingItemEntity
{
    public int Id { get; set; }
    public int? CategoryId { get; set; }
    public string Name { get; set; }
    public string? CategoryName { get; set; }
    public bool IsPurchased { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DatePurchased { get; set; }
}


