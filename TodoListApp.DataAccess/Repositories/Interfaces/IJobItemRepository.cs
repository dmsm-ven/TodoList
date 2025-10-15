using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories.Interfaces;

public interface IJobItemRepository
{
    Task<List<JobItemEntity>> GetAllJobItems(int employeer_id, int takeMaxYears);
    JobItemEntity GetJobItem(int id);
    void DeleteJobItem(int id);
    int AddOrUpdateJobItem(JobItemEntity entity);

    void AddHistoryChanges(int job_item_id, string propertyName, string newValue);
    Task<List<JobItemHistoryEntity>> GetHistoryChangesForJobItem(int job_item_id);
}
