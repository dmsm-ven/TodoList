using System;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess.Repositories;

public class PostgresBCryptUserValidator : IUserRepository
{
    private readonly IDapperDatabaseAccess database;
    private readonly IAppLogger logger;

    public PostgresBCryptUserValidator(IDapperDatabaseAccess database, IAppLogger logger)
    {
        this.database = database;
        this.logger = logger;
    }

    public bool Login(string name, string password, bool savePassword)
    {
        var findedUser = database.GetSingle<AppUserEntity>("SELECT * FROM app_user WHERE Name = @name", new { name });
        if (findedUser == null) { return false; }

        if (findedUser != null)
        {
            if (BCrypt.Net.BCrypt.Verify(password, findedUser.Password))
            {
                if (savePassword)
                {
                    DateTime dt = GetStartupDateTime();
                    string sql = "UPDATE app_user SET SavePasswordTicksState = @ticksAfterTurnOn WHERE Name = @name";
                    database.Execute(sql, new { ticksAfterTurnOn = dt, name });
                }
                else
                {
                    database.Execute("UPDATE app_user SET SavePasswordTicksState = null");
                }

                return true;
            }
        }

        return false;

    }

    public string TryLoginWithSavedPassword()
    {
        string sql = "SELECT * FROM app_user WHERE SavePasswordTicksState IS NOT null LIMIT 1";
        DateTime dt = GetStartupDateTime();
        var user = database.GetSingle<AppUserEntity>(sql);

        if (user?.SavePasswordTicksState != null)
        {
            bool sameTime = Math.Abs((decimal)(dt - user.SavePasswordTicksState.Value).TotalMinutes) <= 1;
            return sameTime ? user.Name : null;
        }
        return null;
    }

    private DateTime GetStartupDateTime()
    {
        return (DateTime.Now - new TimeSpan(10000 * Environment.TickCount64));
    }
}
