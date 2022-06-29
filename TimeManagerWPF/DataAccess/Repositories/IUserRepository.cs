using System;
using System.Collections.Generic;
using System.Globalization;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess.Repositories;

internal interface IUserRepository
{
    bool Login(string name, string password, bool savePassword);

    bool TryLoginWithSavedPassword();
}

public class BCryptUserValidator : IUserRepository
{
    private readonly IDapperDatabaseAccess database;

    public BCryptUserValidator(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public bool Login(string name, string password, bool savePassword)
    {
        var findedUser = database.GetSingle<AppUserEntity>("SELECT * FROM app_user WHERE Name = @name", new { name });
        if (findedUser == null) { return false; }

        if(findedUser != null)
        {
            if(BCrypt.Net.BCrypt.Verify(password, findedUser.Password))
            {
                if (savePassword)
                {
                    DateTime dt = GetStartupDateTime();

                    database.Execute("UPDATE app_user SET SavePasswordTicksState = @ticksAfterTurnOn WHERE Name = @name", 
                        new { ticksAfterTurnOn = dt, name });
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

    public bool TryLoginWithSavedPassword()
    {
        string sql = "SELECT SavePasswordTicksState FROM app_user WHERE SavePasswordTicksState IS NOT  null LIMIT 1";
        DateTime dt = GetStartupDateTime();
        var savedDate = database.GetSingle<DateTime>(sql);

        bool sameTime = Math.Abs((decimal)(dt - savedDate).TotalMinutes) <= 1;
        return sameTime;
    }

    private DateTime GetStartupDateTime()
    {
        return (DateTime.Now - new TimeSpan(10000 * Environment.TickCount64));
    }
}
