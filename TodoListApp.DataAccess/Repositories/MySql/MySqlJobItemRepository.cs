using System.Collections.Generic;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoListApp.DataAccess.Repositories.MySql;

public class MySqlJobItemRepository : IJobItemRepository
{
    private readonly IDapperDatabaseAccess database;

    public MySqlJobItemRepository(IDapperDatabaseAccess database)
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
        string sql = @"INSERT INTO job_item (Id, EmployeerId, Title, Description, Website, IsCompleted, IsPayed, Price, StartDate, EndDate) 
                        VALUES(@Id, @EmployeerId, @Title, @Description, @Website, @IsCompleted, @IsPayed, @Price, @StartDate,  @EndDate) 
                        ON DUPLICATE KEY UPDATE 
                            Title = @Title,
                            EmployeerId = @EmployeerId,
                            Description = @Description,
                            Website = @Website,
                            IsCompleted = @IsCompleted,
                            IsPayed = @IsPayed,
                            Price = @Price,
                            StartDate = @StartDate,
                            EndDate = @EndDate";

        database.Execute(sql, entity);

        int id = entity.id != 0 ?
            entity.id :
            database.GetSingle<int>("SELECT MAX(Id) FROM job_item");

        return id;
    }

    public void DeleteJobItem(int id)
    {
        database.Execute("DELETE FROM job_item WHERE Id = @id", new { id });
    }

    public IEnumerable<JobItemEntity> GetAllJobItems(int employeer_id)
    {
        var items = database.GetList<JobItemEntity>("SELECT * FROM job_item WHERE EmployeerId = @employeer_id", new { employeer_id });
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
        var jobItem = database.GetSingle<JobItemEntity>("SELECT * FROM job_item WHERE Id = @id", new { id });
        return jobItem;
    }
}
