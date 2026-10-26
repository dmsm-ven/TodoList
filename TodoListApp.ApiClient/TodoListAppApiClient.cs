using System.Net;
using System.Net.Http.Json;
using TodoListApp.Core.Dtos;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;

namespace TodoListApp.ApiClient;

/// <summary>Ошибка, возвращенная API (статус != 2xx). Message уже содержит текст, пригодный для показа пользователю.</summary>
public class ApiException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}

public class TodoListAppApiClient(HttpClient client)
{
    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string details = string.Empty;
        try
        {
            details = (await response.Content.ReadAsStringAsync()).Trim();
        }
        catch
        {
            // тело ответа необязательно
        }
        if (details.Length > 300)
        {
            details = details[..300];
        }

        var message = $"Ошибка API: {(int)response.StatusCode} {response.ReasonPhrase}";
        if (details.Length > 0)
        {
            message += $". {details}";
        }
        throw new ApiException(response.StatusCode, message);
    }

    /// <summary>GET с JSON-ответом. Для 204 No Content возвращает default.</summary>
    private async Task<T?> GetAsync<T>(string url)
    {
        using var response = await client.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task UpdateJobItem(UpdateJobDto payload)
    {
        using var response = await client.PutAsJsonAsync($"api/jobs/{payload.Id}", payload);
        await EnsureSuccessAsync(response);
    }
    public async Task<JobItemEntity> AddJobItem(CreateJobDto payload)
    {
        using var response = await client.PostAsJsonAsync("api/jobs", payload);
        await EnsureSuccessAsync(response);
        var item = await response.Content.ReadFromJsonAsync<JobItemEntity>();

        return item ?? throw new Exception("API вернул пустой ответ при создании задачи");
    }
    public async Task<EmployeerEntity> AddEmployeer(CreateEmployeerDto employeer)
    {
        using var response = await client.PostAsJsonAsync($"api/employeers", employeer);
        await EnsureSuccessAsync(response);
        var emp = await response.Content.ReadFromJsonAsync<EmployeerEntity>();

        return emp ?? throw new Exception("API вернул пустой ответ при создании работодателя");
    }
    public async Task AddPayment(EmployeerPaymentPayload payment)
    {
        using var response = await client.PostAsJsonAsync($"api/employeers/{payment.employeer_id}/payments", payment);
        await EnsureSuccessAsync(response);
    }
    public async Task DeleteEmployeer(int id)
    {
        using var response = await client.DeleteAsync($"api/employeers/{id}");
        await EnsureSuccessAsync(response);
    }
    public async Task DeleteJobItem(int id)
    {
        using var response = await client.DeleteAsync($"api/jobs/{id}");
        await EnsureSuccessAsync(response);
    }
    public async Task<List<EmployeerEntity>> GetAllEmployeer()
    {
        var employeers = await GetAsync<List<EmployeerEntity>>("api/employeers");
        return employeers ?? new();
    }
    public async Task<List<JobItemEntity>> GetAllJobItems(int employeer_id)
    {
        var jobs = await GetAsync<List<JobItemEntity>>($"api/jobs?employeer_id={employeer_id}");
        return jobs ?? new();
    }
    public async Task<List<EmployeerPaymentEntity>> GetAllPaymentsForEmployeer(int employeer_id)
    {
        var payments = await GetAsync<List<EmployeerPaymentEntity>>($"api/employeers/{employeer_id}/payments");
        return payments ?? new();
    }
    public async Task<List<LogEntryEntity>> GetLastRows(int take_count)
    {
        var logs = await GetAsync<List<LogEntryEntity>>($"api/logs?take_count={take_count}");
        return logs ?? new();
    }
    public async Task<EmployeerEntity> GetEmployeer(int id)
    {
        var emp = await GetAsync<EmployeerEntity>($"api/employeers/{id}");
        return emp ?? throw new Exception("Employeer not found");
    }
    public async Task<List<JobItemHistoryEntity>> GetHistoryChangesForJobItem(int job_item_id)
    {
        // API возвращает 204, если у задачи еще нет истории изменений
        var changes = await GetAsync<List<JobItemHistoryEntity>>($"api/jobs/{job_item_id}/changes-log");
        return changes ?? new();
    }
    public async Task<JobItemEntity> GetJobItem(int id)
    {
        var job = await GetAsync<JobItemEntity>($"api/jobs/{id}");
        return job ?? throw new Exception("Job item not found");
    }
}
