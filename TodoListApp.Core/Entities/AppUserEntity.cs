namespace TodoListApp.Core.Entities;

public class AppUserEntity
{
    public int id { get; set; }

    public string name { get; set; } = string.Empty;

    public string password { get; set; } = string.Empty;
}
