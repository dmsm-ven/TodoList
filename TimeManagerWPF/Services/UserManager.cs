using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.WPF.DataAccess;
using TodoList.WPF.DataAccess.Entities;
using TodoList.WPF.DataAccess.Repositories;

namespace TodoList.WPF.Models;

public class UserManager
{
    private readonly IUserRepository userRepository;
    private readonly IAppLogger logger;
    public string LoggedUser { get; private set; } = String.Empty;

    public UserManager(IAppLogger logger, IUserRepository userRepository)
    {
        this.logger = logger;
        this.userRepository = userRepository;
    }

    public IReadOnlyList<LogEntryEntity> LogEntries => logger.GetLastRows(100);

    public async Task<bool> Login(string login, string password)
    {
        var loginResult = await userRepository.Login(login, password);

        this.LoggedUser = loginResult ? login : String.Empty;

        if (loginResult)
        {
            logger.WriteLog($"Пользователь '{login}' вошел");
        }
        else
        {
            logger.WriteLog($"Пользователь '{login}' ввел не верный логин/пароль");
        }

        return loginResult;

    }

    internal bool TryLoginWithSavedPassword()
    {
        return false;
    }

    internal void ApplicationClosed()
    {
        logger.WriteLog($"Пользователь '{LoggedUser}' вышел из программы");
    }
}
