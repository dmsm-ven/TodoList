using System.Collections.Generic;
using System.Globalization;

namespace TodoList.WPF.DataAccess.Repositories;

public interface IUserRepository
{
    bool Login(string name, string password);
}
