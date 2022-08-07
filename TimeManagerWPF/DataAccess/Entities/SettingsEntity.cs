using System.ComponentModel.DataAnnotations;

namespace TodoList.WPF.DataAccess.Entities;

public class SettingsEntity
{
    [Key]
    public string Name { get; set; }
    public string Value { get; set; }
}
