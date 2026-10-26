using Microsoft.Extensions.Caching.Memory;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.DataAccess;

public class CachedJobItemRepository : IJobItemRepository
{
    // Страховка на случай, если запись по какой-то причине не была сброшена (например, смена года для фильтра "последние 2 года")
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(5);

    private readonly IJobItemRepository decorator;
    private readonly IMemoryCache cache;

    public CachedJobItemRepository(IJobItemRepository decorator, IMemoryCache cache)
    {
        this.decorator = decorator;
        this.cache = cache;
    }

    private static string JobsOfEmployeerKey(int employeer_id) => $"jobs-emp-{employeer_id}";
    private static string JobKey(int id) => $"job-id{id}";

    // Кэш сбрасывается ПОСЛЕ записи в БД (в finally), иначе параллельный GET успевает закэшировать старые данные
    public async Task<int> AddOrUpdateJobItem(JobItemEntity entity)
    {
        try
        {
            var id = await decorator.AddOrUpdateJobItem(entity);
            cache.Remove(JobKey(id));
            return id;
        }
        finally
        {
            cache.Remove(JobsOfEmployeerKey(entity.employeer_id));
            cache.Remove(JobKey(entity.id));
        }
    }

    public async Task DeleteJobItem(int id)
    {
        // Работодатель нужен, чтобы сбросить его список задач
        var job = await decorator.GetJobItem(id);
        try
        {
            await decorator.DeleteJobItem(id);
        }
        finally
        {
            cache.Remove(JobKey(id));
            if (job is not null)
            {
                cache.Remove(JobsOfEmployeerKey(job.employeer_id));
            }
        }
    }

    public async Task<List<JobItemEntity>> GetAllJobItems(int employeer_id)
    {
        var cacheKey = JobsOfEmployeerKey(employeer_id);

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
        var cacheKey = JobKey(id);
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
}
