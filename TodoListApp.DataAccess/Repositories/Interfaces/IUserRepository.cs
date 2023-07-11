using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace TodoListApp.DataAccess.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> Login(string name, string password);
}
