using System.ComponentModel.DataAnnotations;

namespace TodoList.WPF.DataAccess.Entities;

public class SettingsEntity
{
    [Key]
    public string name { get; set; }
    public string value { get; set; }
}
