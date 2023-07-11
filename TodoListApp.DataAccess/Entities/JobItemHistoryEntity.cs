using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoListApp.DataAccess.Entities;

public class JobItemHistoryEntity
{
    public int history_id { get; set; }
    public int job_item_id { get; set; }
    public DateTime date_time { get; set; }
    public string property_name { get; set; }
    public string new_value { get; set; }
}
