using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess.Repositories;

public interface IAppLogger
{
    void WriteLog(string message);
    List<LogEntryEntity> GetLastRows(int takeCount);
}



