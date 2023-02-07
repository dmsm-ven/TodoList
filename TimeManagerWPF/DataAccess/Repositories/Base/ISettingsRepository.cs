using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.WPF.DataAccess.Repositories;

public interface ISettingsRepository
{
    Dictionary<string, string> GetAll();
    void Set(string name, string value);
}
