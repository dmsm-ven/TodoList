using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.ApiClient;

public class TodoListAppApiClient(HttpClient client) : IEmployeerRepository,
    IJobItemRepository,
    IAppLogger
{
    async Task IJobItemRepository.AddHistoryChanges(JobItemHistoryChangeRequest payload)
    {
        var result = await client.PostAsJsonAsync($"api/jobs/{payload.JobItemId}/changed", payload);
        if(result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
    }

    async Task<int> IJobItemRepository.AddOrUpdateJobItem(JobItemEntity payload)
    {
        var result = await client.PutAsJsonAsync($"api/jobs/{payload.id}/updated", payload);
        if (result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
        var id = await result.Content.ReadFromJsonAsync<int>();
        return id;
    }

    async Task<int> IEmployeerRepository.AddEmployeer(EmployeerEntity entity)
    {
        var result = await client.PostAsJsonAsync($"api/employeers/add", entity);
        if (result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
        var id = await result.Content.ReadFromJsonAsync<int>();
        return id;
    }

    async Task IEmployeerRepository.AddPayment(EmployeerPaymentPayload payment)
    {
        var result = await client.PostAsJsonAsync($"api/employeers/{payment.employeer_id}/payments", payment);
        if (result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }   
    }

    async Task IEmployeerRepository.DeleteEmployeer(int id)
    {
        var result = await client.DeleteAsync($"api/employeers/{id}");
        if (!result.IsSuccessStatusCode)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
    }

    async Task IJobItemRepository.DeleteJobItem(int id)
    {
        var result = await client.DeleteAsync($"api/jobs/{id}");
        if (!result.IsSuccessStatusCode)
        {
            throw new Exception($"Error calling API: {result.StatusCode}");
        }
    }

    async Task<List<EmployeerEntity>> IEmployeerRepository.GetAllEmployeer()
    {
        var employeers = await client.GetFromJsonAsync<List<EmployeerEntity>>("api/employeers");
        return employeers ?? new();
    }

    async Task<List<JobItemEntity>> IJobItemRepository.GetAllJobItems(int employeer_id, int take_max_years)
    {
        var jobs = await client.GetFromJsonAsync<List<JobItemEntity>>($"api/jobs?employeer_id={employeer_id}&take_max_years={take_max_years}");
        return jobs ?? new();
    }

    async Task<List<EmployeerPaymentEntity>> IEmployeerRepository.GetAllPaymentsForEmployeer(int employeer_id)
    {
        var payments = await client.GetFromJsonAsync<List<EmployeerPaymentEntity>>($"api/employeers/{employeer_id}/payments");
        return payments ?? new();
    }

    async Task<List<LogEntryEntity>> IAppLogger.GetLastRows(int take_count)
    {
        var logs = await client.GetFromJsonAsync<List<LogEntryEntity>>($"api/logs?take_count={take_count}");
        return logs ?? new();
    }

    async Task<EmployeerEntity> IEmployeerRepository.GetEmployeer(int id)
    {
        var emp = await client.GetFromJsonAsync<EmployeerEntity?>($"api/employeers/{id}");
        return emp ?? throw new Exception("Employeer not found");
    }

    async Task<List<JobItemHistoryEntity>> IJobItemRepository.GetHistoryChangesForJobItem(int job_item_id)
    {
        var changes = await client.GetFromJsonAsync<List<JobItemHistoryEntity>>($"api/jobs/{job_item_id}/changes-log");
        return changes ?? new();
    }

    async Task<JobItemEntity> IJobItemRepository.GetJobItem(int id)
    {
        var jobs = await client.GetFromJsonAsync<JobItemEntity>($"api/jobs/{id}");
        return jobs ?? throw new Exception("Job item not found");
    }

    Task IAppLogger.WriteLog(string message) => throw new NotSupportedException();
}
