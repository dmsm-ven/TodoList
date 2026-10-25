using Microsoft.Extensions.Caching.Memory;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.DataAccess;

public class CachedEmployeerRepository : IEmployeerRepository
{
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(5);

    private readonly IEmployeerRepository decorator;
    private readonly IMemoryCache cache;

    public CachedEmployeerRepository(IEmployeerRepository decorator, IMemoryCache cache)
    {
        this.decorator = decorator;
        this.cache = cache;
    }

    // Кэш сбрасывается ПОСЛЕ записи в БД (в finally), иначе параллельный GET успевает закэшировать старые данные
    public async Task<int> AddEmployeer(EmployeerEntity entity)
    {
        try
        {
            return await decorator.AddEmployeer(entity);
        }
        finally
        {
            ClearCache();
        }
    }

    public async Task AddPayment(EmployeerPaymentPayload payment)
    {
        try
        {
            await decorator.AddPayment(payment);
        }
        finally
        {
            ClearCache();
        }
    }

    public async Task DeleteEmployeer(int id)
    {
        try
        {
            await decorator.DeleteEmployeer(id);
        }
        finally
        {
            ClearCache();
        }
    }

    public async Task<List<EmployeerEntity>> GetAllEmployeer()
    {
        var cacheKey = "emp-all";
        if (!cache.TryGetValue<List<EmployeerEntity>>(cacheKey, out var list) || list is null)
        {
            list = await decorator.GetAllEmployeer();
            if (list is not null)
            {
                cache.Set(cacheKey, list, CacheLifetime);
            }
        }
        return list ?? new();
    }

    public async Task<List<EmployeerPaymentEntity>> GetAllPaymentsForEmployeer(int employeer_id)
    {
        var cacheKey = $"emp-payments-{employeer_id}";
        if (!cache.TryGetValue<List<EmployeerPaymentEntity>>(cacheKey, out var payments) || payments is null)
        {
            payments = await decorator.GetAllPaymentsForEmployeer(employeer_id);
            if (payments is not null)
            {
                cache.Set(cacheKey, payments, CacheLifetime);
            }
        }
        return payments ?? new();
    }

    public async Task<EmployeerEntity> GetEmployeer(int id)
    {
        var cacheKey = $"emp-{id}";
        if (!cache.TryGetValue<EmployeerEntity>(cacheKey, out var emp) || emp is null)
        {
            emp = await decorator.GetEmployeer(id);

            // null не кэшируем: иначе после создания работодателя он еще какое-то время "не существует"
            if (emp is not null)
            {
                cache.Set(cacheKey, emp, CacheLifetime);
            }
        }
        return emp!;
    }

    private void ClearCache()
    {
        if (cache is MemoryCache memoryCache)
        {
            memoryCache.Clear();
        }
    }
}
