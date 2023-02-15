using System.Collections.Generic;
using TodoList.DataAccess;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess;

public class PostgresJobItemRepository : IJobItemRepository
{
    private readonly IDapperDatabaseAccess database;

    public PostgresJobItemRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public void AddHistoryChanges(int job_item_id, string propertyName, string newValue)
    {
        string sql = @"INSERT INTO job_item_history (job_item_id, property_name, new_value) VALUES
                                                    (@job_item_id, @propertyName, @newValue)";
        database.Execute(sql, new { job_item_id, propertyName, newValue });
    }

    public int AddOrUpdateJobItem(JobItemEntity entity)
    {
        int max_id = entity.id != 0 ?
            entity.id :
            database.GetSingle<int>("SELECT MAX(id) FROM job_item") + 1;
        if (entity.id == 0)
        {
            entity.id = max_id;
        }

        string sql = @"INSERT INTO job_item (id, employeer_id, title, description, website, is_completed, is_payed, price, start_date, end_date) 
                        VALUES(@id, @employeer_id, @title, @description, @website, @is_completed, @is_payed, @price, @start_date,  @end_date) 
                        ON CONFLICT (id) DO UPDATE 
                            SET title = @title,
                                employeer_id = @employeer_id,
                                description = @description,
                                website = @website,
                                is_completed = @is_completed,
                                is_payed = @is_payed,
                                price = @price,
                                start_date = @start_date,
                                end_date = @end_date";

        database.Execute(sql, entity);

        return max_id;
    }

    public void DeleteJobItem(int id)
    {
        database.Execute("DELETE FROM job_item WHERE id = @id", new { id });
    }

    public IEnumerable<JobItemEntity> GetAllJobItems(int employeer_id)
    {
        var items = database.GetList<JobItemEntity>("SELECT * FROM job_item WHERE employeer_id = @employeer_id", new { employeer_id });
        return items;
    }

    public IEnumerable<JobItemHistoryEntity> GetHistoryChangesForJobItem(int job_item_id)
    {
        string sql = @"SELECT * 
                        FROM job_item_history 
                        WHERE job_item_id = @job_item_id
                        ORDER BY date_time DESC";
        var items = database.GetList<JobItemHistoryEntity>(sql, new { job_item_id });
        return items;
    }

    public JobItemEntity GetJobItem(int id)
    {
        var jobItem = database.GetSingle<JobItemEntity>("SELECT * FROM job_item WHERE id = @id", new { id });
        return jobItem;
    }
}
