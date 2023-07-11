using System;
using System.Threading.Tasks;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoListApp.DataAccess.Repositories.Postgres.Repositories;

public class PostgresBCryptUserValidator : IUserRepository
{
    private readonly IDapperDatabaseAccess database;
    private readonly IAppLogger logger;

    public PostgresBCryptUserValidator(IDapperDatabaseAccess database, IAppLogger logger)
    {
        this.database = database;
        this.logger = logger;
    }

    public async Task<bool> Login(string name, string password)
    {
        var findedUser = await database.GetSingleAsync<AppUserEntity>("SELECT * FROM app_user WHERE name = @name", new { name });

        return findedUser != null && BCrypt.Net.BCrypt.Verify(password, findedUser.password);

    }
}
