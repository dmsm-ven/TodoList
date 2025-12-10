namespace TodoListApp.Core.Entities;

public class JobItemHistoryEntity
{
    public int history_id { get; set; }
    public int job_item_id { get; set; }
    public DateTimeOffset date_time { get; set; }
    public string property_name { get; set; }
    public string new_value { get; set; }
}
