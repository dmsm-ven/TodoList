using System.ComponentModel.DataAnnotations;

namespace TodoListApp.DataAccess.Entities;

public class SettingsEntity
{
    [Key]
    public string name { get; set; }
    public string value { get; set; }
}
