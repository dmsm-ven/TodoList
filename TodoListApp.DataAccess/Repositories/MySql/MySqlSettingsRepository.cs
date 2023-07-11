using System.Collections.Generic;
using System.Linq;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoListApp.DataAccess.Repositories.MySql;

public class MySqlSettingsRepository : ISettingsRepository
{
    private readonly IDapperDatabaseAccess database;

    public MySqlSettingsRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public IReadOnlyDictionary<string, string> GetAll()
    {
        var settings = database.GetList<SettingsEntity>("SELECT * FROM setting_item");

        return settings.ToDictionary(i => i.name, i => i.value);
    }

    public void Set(string name, string value)
    {
        string sql = @"INSERT INTO setting_item (`Name`, `Value`) 
                        VALUES(@setting_name, @setting_value) 
                        ON DUPLICATE KEY UPDATE `Value` = @setting_value";

        database.Execute(sql, new { setting_name = name, setting_value = value });
    }
}
