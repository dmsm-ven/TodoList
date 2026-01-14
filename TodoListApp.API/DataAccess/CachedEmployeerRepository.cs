using Microsoft.Extensions.Caching.Memory;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.DataAccess;

public class CachedEmployeerRepository : IEmployeerRepository
{
    private readonly IEmployeerRepository decorator;
    private readonly IMemoryCache cache;

    public CachedEmployeerRepository(IEmployeerRepository decorator, IMemoryCache cache)
    {
        this.decorator = decorator;
        this.cache = cache;
    }

    public Task<int> AddEmployeer(EmployeerEntity entity)
    {
        cache.Remove("emp-all");
        cache.Remove($"emp-{entity.id}");
        return decorator.AddEmployeer(entity);
    }

    public Task AddPayment(EmployeerPaymentPayload payment)
    {
        cache.Remove($"emp-payments-{payment.employeer_id}");
        return decorator.AddPayment(payment);
    }

    public Task DeleteEmployeer(int id)
    {
        cache.Remove("emp-all");
        cache.Remove($"emp-{id}");
        return decorator.DeleteEmployeer(id);
    }

    public async Task<List<EmployeerEntity>> GetAllEmployeer()
    {
        var cacheKey = "emp-all";
        if (!cache.TryGetValue<List<EmployeerEntity>>(cacheKey, out var list))
        {
            list = await decorator.GetAllEmployeer();
            cache.Set(cacheKey, list);
        }
        return list ?? new();
    }

    public async Task<List<EmployeerPaymentEntity>> GetAllPaymentsForEmployeer(int employeer_id)
    {
        var cacheKey = $"emp-payments-{employeer_id}";
        if (!cache.TryGetValue<List<EmployeerPaymentEntity>>(cacheKey, out var payments))
        {
            payments = await decorator.GetAllPaymentsForEmployeer(employeer_id);
            cache.Set(cacheKey, payments);
        }
        return payments ?? new();
    }

    public async Task<EmployeerEntity> GetEmployeer(int id)
    {
        var cacheKey = $"emp-{id}";
        if (!cache.TryGetValue<EmployeerEntity>(cacheKey, out var emp))
        {
            emp = await decorator.GetEmployeer(id);
            cache.Set(cacheKey, emp);
        }
        return emp;
    }
}
