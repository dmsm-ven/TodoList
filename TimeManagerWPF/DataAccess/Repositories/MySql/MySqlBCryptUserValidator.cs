using System;
using System.Threading.Tasks;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess.Repositories;

public class MySqlBCryptUserValidator : IUserRepository
{
    private readonly IDapperDatabaseAccess database;
    private readonly IAppLogger logger;

    public MySqlBCryptUserValidator(IDapperDatabaseAccess database, IAppLogger logger)
    {
        this.database = database;
        this.logger = logger;
    }

    public async Task<bool> Login(string name, string password)
    {
        var findedUser = await database.GetSingleAsync<AppUserEntity>("SELECT * FROM app_user WHERE Name = @name", new { name });
        if (findedUser == null) { return false; }

        if (findedUser != null)
        {
            if (BCrypt.Net.BCrypt.Verify(password, findedUser.password))
            {
                return true;
            }
        }

        return false;

    }

    private DateTime GetStartupDateTime()
    {
        return (DateTime.Now - new TimeSpan(10000 * Environment.TickCount64));
    }
}
