using System.Collections.Generic;
using TodoList.DataAccess;

namespace TodoList.WPF.DataAccess;

public interface IJobItemRepository
{
    IEnumerable<JobItemEntity> GetAllJobItems(int employeer_id);
    JobItemEntity GetJobItem(int id);
    void DeleteJobItem(int id);
    int AddOrUpdateJobItem(JobItemEntity entity);
}

public class JobItemRepository : IJobItemRepository
{
    private readonly IDapperDatabaseAccess database;

    public JobItemRepository(IDapperDatabaseAccess database)
    {
        this.database = database;
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

        int id = entity.Id != 0 ? 
            entity.Id : 
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

    public JobItemEntity GetJobItem(int id)
    {
        var jobItem = database.GetSingle<JobItemEntity>("SELECT * FROM job_item WHERE Id = @id", new { id });
        return jobItem;
    }
}
