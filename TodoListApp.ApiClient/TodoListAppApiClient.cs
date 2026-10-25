using System.Net;
using System.Net.Http.Json;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.ApiClient;

/// <summary>Ошибка, возвращенная API (статус != 2xx). Message уже содержит текст, пригодный для показа пользователю.</summary>
public class ApiException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}

public class TodoListAppApiClient(HttpClient client) : IEmployeerRepository,
    IJobItemRepository,
    IAppLogger
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

    async Task<int> IJobItemRepository.AddOrUpdateJobItem(JobItemEntity payload)
    {
        // Новая задача (id = 0) создается через POST, существующая обновляется через PUT
        using var response = payload.id == 0
            ? await client.PostAsJsonAsync("api/jobs", payload)
            : await client.PutAsJsonAsync($"api/jobs/{payload.id}/updated", payload);

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<int>();
    }
    async Task<int> IEmployeerRepository.AddEmployeer(EmployeerEntity entity)
    {
        using var response = await client.PostAsJsonAsync($"api/employeers/add", entity);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<int>();
    }
    async Task IEmployeerRepository.AddPayment(EmployeerPaymentPayload payment)
    {
        using var response = await client.PostAsJsonAsync($"api/employeers/{payment.employeer_id}/payments", payment);
        await EnsureSuccessAsync(response);
    }
    async Task IEmployeerRepository.DeleteEmployeer(int id)
    {
        using var response = await client.DeleteAsync($"api/employeers/{id}");
        await EnsureSuccessAsync(response);
    }
    async Task IJobItemRepository.DeleteJobItem(int id)
    {
        using var response = await client.DeleteAsync($"api/jobs/{id}");
        await EnsureSuccessAsync(response);
    }
    async Task<List<EmployeerEntity>> IEmployeerRepository.GetAllEmployeer()
    {
        var employeers = await GetAsync<List<EmployeerEntity>>("api/employeers");
        return employeers ?? new();
    }
    async Task<List<JobItemEntity>> IJobItemRepository.GetAllJobItems(int employeer_id)
    {
        var jobs = await GetAsync<List<JobItemEntity>>($"api/jobs?employeer_id={employeer_id}");
        return jobs ?? new();
    }
    async Task<List<EmployeerPaymentEntity>> IEmployeerRepository.GetAllPaymentsForEmployeer(int employeer_id)
    {
        var payments = await GetAsync<List<EmployeerPaymentEntity>>($"api/employeers/{employeer_id}/payments");
        return payments ?? new();
    }
    async Task<List<LogEntryEntity>> IAppLogger.GetLastRows(int take_count)
    {
        var logs = await GetAsync<List<LogEntryEntity>>($"api/logs?take_count={take_count}");
        return logs ?? new();
    }
    async Task<EmployeerEntity> IEmployeerRepository.GetEmployeer(int id)
    {
        var emp = await GetAsync<EmployeerEntity>($"api/employeers/{id}");
        return emp ?? throw new Exception("Employeer not found");
    }
    async Task<List<JobItemHistoryEntity>> IJobItemRepository.GetHistoryChangesForJobItem(int job_item_id)
    {
        // API возвращает 204, если у задачи еще нет истории изменений
        var changes = await GetAsync<List<JobItemHistoryEntity>>($"api/jobs/{job_item_id}/changes-log");
        return changes ?? new();
    }
    async Task<JobItemEntity> IJobItemRepository.GetJobItem(int id)
    {
        var job = await GetAsync<JobItemEntity>($"api/jobs/{id}");
        return job ?? throw new Exception("Job item not found");
    }
    async Task IAppLogger.WriteLog(string userIp, string path, string message) => throw new NotSupportedException();
}
