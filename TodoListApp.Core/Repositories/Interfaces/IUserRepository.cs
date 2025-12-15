namespace TodoListApp.Core.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> Login(string name, string password);
}
