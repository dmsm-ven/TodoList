using System;
using TodoListApp.Core.Dtos;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;
using TodoListApp.Desktop.ViewModels;

namespace TodoListApp.Desktop.Infrastructure.MapperHelper;

public static class JobItemMapperHelper
{
    public static JobItemViewModel ToViewModel(this JobItemEntity jobItem)
    {
        return new JobItemViewModel
        {
            Id = jobItem.id,
            Title = jobItem.title,
            Price = jobItem.price,
            Website = jobItem.website ?? string.Empty,
            Description = jobItem.description ?? string.Empty,
            IsCompleted = jobItem.is_completed,
            IsPayed = jobItem.is_payed,
            StartDate = jobItem.start_date.DateTime,
            EndDate = jobItem.end_date,
            EmployeerId = jobItem.employeer_id,
        };
    }

    public static JobItemEntity ToEntity(this JobItemViewModel jobItem)
    {
        return new JobItemEntity
        {
            id = jobItem.Id,
            title = jobItem.Title,
            description = jobItem.Description,
            is_completed = jobItem.IsCompleted,
            is_payed = jobItem.IsPayed,
            start_date = jobItem.StartDate.Value,
            end_date = jobItem.EndDate,
            employeer_id = jobItem.EmployeerId,
            price = jobItem.Price,
            website = jobItem.Website
        };
    }
    public static UpdateJobDto ToUpdateJobItem(this JobItemViewModel jobItem)
    {
        return new UpdateJobDto
        {
            Id = jobItem.Id,
            EmployeerId = jobItem.EmployeerId,
            Title = jobItem.Title,
            Description = jobItem.Description,
            IsCompleted = jobItem.IsCompleted,
            IsPayed = jobItem.IsPayed,
            StartDate = jobItem.StartDate.Value,
            EndDate = jobItem.EndDate != null ? DateOnly.FromDateTime(jobItem.EndDate.Value.UtcDateTime) : null,
            Price = jobItem.Price,
            Website = jobItem.Website
        };
    }

    public static CreateJobDto ToCreateJobItem(this JobItemViewModel jobItem)
    {
        return new CreateJobDto
        {
            Title = jobItem.Title,
            Description = jobItem.Description,
            StartDate = jobItem.StartDate.Value,
            Price = jobItem.Price,
            Website = jobItem.Website,
            EmployeerId = jobItem.EmployeerId
        };
    }

    public static JobItemHistoryLineModel ToModel(this JobItemHistoryEntity jobItemHistory)
    {
        return new JobItemHistoryLineModel()
        {
            LocalTime = jobItemHistory.date_time.LocalDateTime,
            ChangedPropertyName = jobItemHistory.property_name,
            NewValue = jobItemHistory.new_value,
        };
    }
}
