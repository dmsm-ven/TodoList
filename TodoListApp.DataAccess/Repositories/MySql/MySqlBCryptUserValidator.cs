using TodoListApp.Core;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoListApp.DataAccess.Repositories.MySql;

public class MySqlBCryptUserValidator : IUserRepository
{
    private readonly IDapperDatabaseAccess database;
    private readonly IAppLogger logger;
    private readonly IUserDataEncryptValidator encryptValidator;

    public MySqlBCryptUserValidator(IDapperDatabaseAccess database, IAppLogger logger, IUserDataEncryptValidator encryptValidator)
    {
        this.database = database;
        this.logger = logger;
        this.encryptValidator = encryptValidator;
    }

    public async Task<bool> Login(string name, string password)
    {
        var findedUser = await database.GetSingleAsync<AppUserEntity>("SELECT * FROM app_user WHERE Name = @name", new { name });
        if (findedUser == null)
        {
            return false;
        }

        if (findedUser != null)
        {
            return encryptValidator.Verify(password, findedUser.password);
        }

        return false;

    }

    private DateTime GetStartupDateTime()
    {
        return DateTime.Now - new TimeSpan(10000 * Environment.TickCount64);
    }
}
