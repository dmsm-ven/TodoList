using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace TodoList.WPF.DataAccess.Repositories;

public interface IUserRepository
{
    Task<bool> Login(string name, string password);
}
