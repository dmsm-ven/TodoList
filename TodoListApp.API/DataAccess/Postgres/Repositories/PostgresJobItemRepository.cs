using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.Core.Repositories.Postgres.Repositories;

public class PostgresJobItemRepository : IJobItemRepository
{
    private readonly IDapperDatabaseAccess database;

    public PostgresJobItemRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
    }

    public async Task AddHistoryChanges(JobItemHistoryChangeRequest data)
    {
        string sql = @"INSERT INTO job_item_history (job_item_id, property_name, new_value) VALUES
                                                    (@JobItemId, @PropertyName, @NewValue)";
        database.Execute(sql, data);
    }

    public async Task<int> AddOrUpdateJobItem(JobItemEntity entity)
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

        await database.ExecuteAsync(sql, entity);

        return max_id;
    }

    public async Task DeleteJobItem(int id)
    {
        await database.ExecuteAsync("DELETE FROM job_item WHERE id = @id", new { id });
    }

    public async Task<List<JobItemEntity>> GetAllJobItems(int employeer_id, int takeMaxYears, bool only_this_month)
    {
        string sql = @"SELECT * 
                       FROM job_item 
                       WHERE employeer_id = @employeer_id";
        if (only_this_month)
        {
            sql += " AND date_part('month', start_date) = date_part('month', CURRENT_DATE)"
                 + " AND date_part('year', start_date) = date_part('year', CURRENT_DATE)";
        }
        else if (takeMaxYears > 0)
        {
            sql += " AND date_part('year', start_date) > (date_part('year', CURRENT_DATE) - @takeMaxYears)";
        }

        var items = await database.GetListAsync<JobItemEntity>(sql, new { employeer_id, takeMaxYears });
        return items;
    }

    public async Task<List<JobItemHistoryEntity>> GetHistoryChangesForJobItem(int job_item_id)
    {
        string sql = @"SELECT * 
                        FROM job_item_history 
                        WHERE job_item_id = @job_item_id
                        ORDER BY date_time DESC";
        var items = await database.GetListAsync<JobItemHistoryEntity>(sql, new { job_item_id });
        return items;
    }

    public async Task<JobItemEntity> GetJobItem(int id)
    {
        var jobItem = await database.GetSingleAsync<JobItemEntity>("SELECT * FROM job_item WHERE id = @id", new { id });
        return jobItem;
    }
}
