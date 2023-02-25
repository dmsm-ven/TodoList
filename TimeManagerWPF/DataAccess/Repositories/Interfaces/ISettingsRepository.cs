using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.WPF.DataAccess;

public interface ISettingsRepository
{
    IReadOnlyDictionary<string, string> GetAll();
    void Set(string name, string value);
}
