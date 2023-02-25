using System.Collections.Generic;
using TodoList.DataAccess;
using TodoList.WPF.DataAccess.Entities;

namespace TodoList.WPF.DataAccess;

public interface IJobItemRepository
{
    IEnumerable<JobItemEntity> GetAllJobItems(int employeer_id);
    JobItemEntity GetJobItem(int id);
    void DeleteJobItem(int id);
    int AddOrUpdateJobItem(JobItemEntity entity);

    void AddHistoryChanges(int job_item_id, string propertyName, string newValue);
    IEnumerable<JobItemHistoryEntity> GetHistoryChangesForJobItem(int job_item_id);
}
