using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Core.Entities;

public class SettingsEntity
{
    [Key]
    public string name { get; set; }
    public string value { get; set; }
}
