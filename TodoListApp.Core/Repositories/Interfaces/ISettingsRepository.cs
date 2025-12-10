using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TodoListApp.Core.Repositories.Interfaces;

public interface ISettingsRepository
{
    IReadOnlyDictionary<string, string> GetAll();
    void Set(string name, string value);
}
