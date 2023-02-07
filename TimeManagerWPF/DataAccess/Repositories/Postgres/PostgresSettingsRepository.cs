using System.Collections.Generic;
using System.Linq;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess.Repositories;

public class PostgresSettingsRepository : ISettingsRepository
{
    private readonly IDapperDatabaseAccess database;

    public PostgresSettingsRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public Dictionary<string, string> GetAll()
    {
        var settings = database.GetList<SettingsEntity>("SELECT * FROM setting_item");

        return settings.ToDictionary(i => i.Name, i => i.Value);
    }

    public void Set(string name, string value)
    {
        string sql = @"INSERT INTO setting_item (`Name`, `Value`) 
                        VALUES(@setting_name, @setting_value) 
                        ON DUPLICATE KEY UPDATE `Value` = @setting_value";

        database.Execute(sql, new { setting_name = name, setting_value = value });
    }
}
