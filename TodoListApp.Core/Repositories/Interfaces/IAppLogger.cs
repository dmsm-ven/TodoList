using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoListApp.Core.Entities;

namespace TodoListApp.Core.Repositories.Interfaces;

public interface IAppLogger
{
    void WriteLog(string message);
    List<LogEntryEntity> GetLastRows(int takeCount);
}



