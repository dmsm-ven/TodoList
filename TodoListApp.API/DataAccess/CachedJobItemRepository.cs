using Microsoft.Extensions.Caching.Memory;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.DataAccess;

public class CachedJobItemRepository : IJobItemRepository
{
    // Страховка на случай, если кэш по какой-то причине не был сброшен (например, смена года для фильтра "последние 2 года")
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(5);

    private readonly IJobItemRepository decorator;
    private readonly IMemoryCache cache;

    public CachedJobItemRepository(IJobItemRepository decorator, IMemoryCache cache)
    {
        this.decorator = decorator;
        this.cache = cache;
    }

    // Кэш сбрасывается ПОСЛЕ записи в БД (в finally), иначе параллельный GET успевает закэшировать старые данные
    public async Task<int> AddOrUpdateJobItem(JobItemEntity entity)
    {
        try
        {
            return await decorator.AddOrUpdateJobItem(entity);
        }
        finally
        {
            ClearCache();
        }
    }

    public async Task DeleteJobItem(int id)
    {
        try
        {
            await decorator.DeleteJobItem(id);
        }
        finally
        {
            ClearCache();
        }
    }

    public async Task<List<JobItemEntity>> GetAllJobItems(int employeer_id)
    {
        var cacheKey = $"jobs-emp-{employeer_id}";

        if (!cache.TryGetValue<List<JobItemEntity>>(cacheKey, out var items) || items is null)
        {
            items = await decorator.GetAllJobItems(employeer_id);
            if (items is not null)
            {
                cache.Set(cacheKey, items, CacheLifetime);
            }
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
        if (!cache.TryGetValue<JobItemEntity>(cacheKey, out var entity) || entity is null)
        {
            entity = await decorator.GetJobItem(id);

            // null не кэшируем: иначе после создания задачи она еще какое-то время "не существует"
            if (entity is not null)
            {
                cache.Set(cacheKey, entity, CacheLifetime);
            }
        }
        return entity!;
    }

    private void ClearCache()
    {
        if (cache is MemoryCache memoryCache)
        {
            memoryCache.Clear();
        }
    }
}
