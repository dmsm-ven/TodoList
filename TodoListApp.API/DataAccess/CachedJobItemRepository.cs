using Microsoft.Extensions.Caching.Memory;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.DataAccess;

public class CachedJobItemRepository : IJobItemRepository
{
    private readonly IJobItemRepository decorator;
    private readonly IMemoryCache cache;

    public CachedJobItemRepository(IJobItemRepository decorator, IMemoryCache cache)
    {
        this.decorator = decorator;
        this.cache = cache;
    }

    public Task<int> AddOrUpdateJobItem(JobItemEntity entity)
    {
        cache.Remove($"jobs-emp-{entity.employeer_id}");
        return decorator.AddOrUpdateJobItem(entity);
    }

    public async Task DeleteJobItem(int id)
    {
        var job = await decorator.GetJobItem(id);
        cache.Remove($"jobs-emp-{job.employeer_id}");
        await decorator.DeleteJobItem(id);
    }

    public async Task<List<JobItemEntity>> GetAllJobItems(int employeer_id)
    {
        var cacheKey = $"jobs-emp-{employeer_id}";

        if (!cache.TryGetValue<List<JobItemEntity>>(cacheKey, out var items))
        {
            items = await decorator.GetAllJobItems(employeer_id);
            cache.Set(cacheKey, items);
        }

        return items ?? new();
    }

    public Task<List<JobItemHistoryEntity>> GetHistoryChangesForJobItem(int job_item_id)
    {
        return decorator.GetHistoryChangesForJobItem(job_item_id);
    }

    public async Task<JobItemEntity> GetJobItem(int id)
    {
        var cacheKey = $"job-id{id}";
        if (!cache.TryGetValue<JobItemEntity>(cacheKey, out var entity))
        {
            entity = await decorator.GetJobItem(id);
            cache.Set(cacheKey, entity);
        }
        return entity;
    }
}
