using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;

namespace TodoListApp.Core.Repositories.Interfaces;


public interface IJobItemRepository
{
    Task<List<JobItemEntity>> GetAllJobItems(int employeer_id, int takeMaxYears, bool only_this_month);
    Task<JobItemEntity> GetJobItem(int id);
    Task DeleteJobItem(int id);
    Task<int> AddOrUpdateJobItem(JobItemEntity entity);
    Task AddHistoryChanges(JobItemHistoryChangeRequest request);
    Task<List<JobItemHistoryEntity>> GetHistoryChangesForJobItem(int job_item_id);
}
