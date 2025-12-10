using System.Collections.Generic;
using System.Threading.Tasks;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.Desktop.Services;

public class UserManager
{
    private readonly IUserRepository userRepository;
    private readonly IAppLogger logger;
    public string LoggedUser { get; private set; } = string.Empty;

    public UserManager(IAppLogger logger, IUserRepository userRepository)
    {
        this.logger = logger;
        this.userRepository = userRepository;
    }

    public IReadOnlyList<LogEntryEntity> LogEntries => logger.GetLastRows(100);

    public async Task<bool> Login(string login, string password)
    {
        var loginResult = await userRepository.Login(login, password);

        LoggedUser = loginResult ? login : string.Empty;

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
