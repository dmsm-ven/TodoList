using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.ApiClient;

public class TodoListAppApiClient(HttpClient client) : IEmployeerPaymentRepository,
    IEmployeerRepository,
    IJobItemRepository,
    ISettingsRepository,
    IUserRepository,
    IAppLogger
{
    void IJobItemRepository.AddHistoryChanges(int job_item_id, string propertyName, string newValue)
    {
        throw new NotImplementedException();
    }

    int IEmployeerRepository.AddOrUpdateEmployeer(EmployeerEntity entity)
    {
        throw new NotImplementedException();
    }

    int IJobItemRepository.AddOrUpdateJobItem(JobItemEntity entity)
    {
        throw new NotImplementedException();
    }

    void IEmployeerPaymentRepository.AddPayment(EmployeerPaymentEntity payment)
    {
        throw new NotImplementedException();
    }

    void IEmployeerRepository.DeleteEmployeer(int id)
    {
        throw new NotImplementedException();
    }

    void IJobItemRepository.DeleteJobItem(int id)
    {
        throw new NotImplementedException();
    }

    IReadOnlyDictionary<string, string> ISettingsRepository.GetAll()
    {
        throw new NotImplementedException();
    }

    async Task<List<EmployeerEntity>> IEmployeerRepository.GetAllEmployeer()
    {
        // OK ENTRY 1
        var employeers = await client.GetFromJsonAsync<List<EmployeerEntity>>("api/employeers");
        return employeers ?? new();
    }

    async Task<List<JobItemEntity>> IJobItemRepository.GetAllJobItems(int employeer_id, int take_max_years)
    {
        // OK ENTRY 2
        var jobs = await client.GetFromJsonAsync<List<JobItemEntity>>($"api/jobs?employeer_id={employeer_id}&take_max_years={take_max_years}");
        return jobs ?? new();
    }

    async Task<List<EmployeerPaymentEntity>> IEmployeerPaymentRepository.GetAllPaymentsForEmployeer(int employeer_id)
    {
        // OK ENTRY 3
        var payments = await client.GetFromJsonAsync<List<EmployeerPaymentEntity>>($"api/payments/{employeer_id}");
        return payments ?? new();
    }

    EmployeerEntity IEmployeerRepository.GetEmployeer(int id)
    {
        throw new NotImplementedException();
    }

    Task<List<JobItemHistoryEntity>> IJobItemRepository.GetHistoryChangesForJobItem(int job_item_id)
    {
        throw new NotImplementedException();
    }

    JobItemEntity IJobItemRepository.GetJobItem(int id)
    {
        throw new NotImplementedException();
    }

    List<LogEntryEntity> IAppLogger.GetLastRows(int takeCount)
    {
        throw new NotImplementedException();
    }

    Task<bool> IUserRepository.Login(string name, string password)
    {
        throw new NotImplementedException();
    }

    void ISettingsRepository.Set(string name, string value)
    {
        throw new NotImplementedException();
    }

    void IAppLogger.WriteLog(string message)
    {
        throw new NotImplementedException();
    }
}
