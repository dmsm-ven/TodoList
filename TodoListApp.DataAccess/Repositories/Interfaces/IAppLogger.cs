using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories.Interfaces;

public interface IAppLogger
{
    void WriteLog(string message);
    List<LogEntryEntity> GetLastRows(int takeCount);
}



