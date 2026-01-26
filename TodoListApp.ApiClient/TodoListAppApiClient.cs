using System.Net.Http.Json;
using TodoListApp.Core.Dtos;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;

namespace TodoListApp.ApiClient;

public class TodoListAppApiClient(HttpClient client)
{
    public async Task UpdateJobItem(UpdateJobDto payload)
    {
        var result = await client.PutAsJsonAsync($"api/jobs/{payload.Id}", payload);
        if (!result.IsSuccessStatusCode)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
    }
    public async Task<int> AddJobItem(CreateJobDto payload)
    {
        var result = await client.PostAsJsonAsync("api/jobs", payload);
        if (!result.IsSuccessStatusCode)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
        var id = await result.Content.ReadFromJsonAsync<int>();
        return id;
    }
    public async Task<int> AddEmployeer(CreateEmployeerDto employeer)
    {
        var result = await client.PostAsJsonAsync($"api/employeers", employeer);
        if (result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
        var id = await result.Content.ReadFromJsonAsync<int>();
        return id;
    }
    public async Task AddPayment(EmployeerPaymentPayload payment)
    {
        var result = await client.PostAsJsonAsync($"api/employeers/{payment.employeer_id}/payments", payment);
        if (result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
    }
    public async Task DeleteEmployeer(int id)
    {
        var result = await client.DeleteAsync($"api/employeers/{id}");
        if (!result.IsSuccessStatusCode)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
    }
    public async Task DeleteJobItem(int id)
    {
        var result = await client.DeleteAsync($"api/jobs/{id}");
        if (!result.IsSuccessStatusCode)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
    }
    public async Task<List<EmployeerEntity>> GetAllEmployeer()
    {
        var employeers = await client.GetFromJsonAsync<List<EmployeerEntity>>("api/employeers");
        return employeers ?? new();
    }
    public async Task<List<JobItemEntity>> GetAllJobItems(int employeer_id)
    {
        var jobs = await client.GetFromJsonAsync<List<JobItemEntity>>($"api/jobs?employeer_id={employeer_id}");
        return jobs ?? new();
    }
    public async Task<List<EmployeerPaymentEntity>> GetAllPaymentsForEmployeer(int employeer_id)
    {
        var payments = await client.GetFromJsonAsync<List<EmployeerPaymentEntity>>($"api/employeers/{employeer_id}/payments");
        return payments ?? new();
    }
    public async Task<List<LogEntryEntity>> GetLastRows(int take_count)
    {
        var logs = await client.GetFromJsonAsync<List<LogEntryEntity>>($"api/logs?take_count={take_count}");
        return logs ?? new();
    }
    public async Task<EmployeerEntity> GetEmployeer(int id)
    {
        var emp = await client.GetFromJsonAsync<EmployeerEntity?>($"api/employeers/{id}");
        return emp ?? throw new Exception("Employeer not found");
    }
    public async Task<List<JobItemHistoryEntity>> GetHistoryChangesForJobItem(int job_item_id)
    {
        var changes = await client.GetFromJsonAsync<List<JobItemHistoryEntity>>($"api/jobs/{job_item_id}/changes-log");
        return changes ?? new();
    }
    public async Task<JobItemEntity> GetJobItem(int id)
    {
        var jobs = await client.GetFromJsonAsync<JobItemEntity>($"api/jobs/{id}");
        return jobs ?? throw new Exception("Job item not found");
    }
}
