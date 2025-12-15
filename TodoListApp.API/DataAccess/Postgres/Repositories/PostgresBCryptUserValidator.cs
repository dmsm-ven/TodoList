using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.Core.Repositories.Postgres.Repositories;

public class PostgresBCryptUserValidator : IUserRepository
{
    private readonly IDapperDatabaseAccess database;
    private readonly IAppLogger logger;
    private readonly IUserDataEncryptValidator encryptValidator;

    public PostgresBCryptUserValidator(IDapperDatabaseAccess database, IAppLogger logger, IUserDataEncryptValidator encryptValidator)
    {
        this.database = database;
        this.logger = logger;
        this.encryptValidator = encryptValidator;
    }

    public async Task<bool> Login(string name, string password)
    {
        var findedUser = await database.GetSingleAsync<AppUserEntity>("SELECT * FROM app_user WHERE name = @name", new { name });

        return findedUser != null && encryptValidator.Verify(password, findedUser.password);

    }
}
