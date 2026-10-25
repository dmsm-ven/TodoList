using System;
using System.Globalization;
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
            // API отдает UTC, берем локальное время (раньше .DateTime давал UTC-часы, из-за чего дата уезжала на день назад)
            StartDate = jobItem.start_date.LocalDateTime,
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
            start_date = jobItem.StartDate ?? throw new InvalidOperationException("Не указана дата создания задачи"),
            end_date = jobItem.EndDate,
            employeer_id = jobItem.EmployeerId,
            price = jobItem.Price,
            website = jobItem.Website
        };
    }

    public static JobItemHistoryLineModel ToModel(this JobItemHistoryEntity jobItemHistory)
    {
        var propertyName = jobItemHistory.property_name;

        return new JobItemHistoryLineModel()
        {
            LocalTime = jobItemHistory.date_time.LocalDateTime,
            ChangedPropertyName = JobItemViewModel.PropertyNameToTitle.TryGetValue(propertyName, out var title) ? title : propertyName,
            NewValue = FormatHistoryValue(propertyName, jobItemHistory.new_value),
        };
    }

    private static string FormatHistoryValue(string propertyName, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "—";
        }

        if (bool.TryParse(value, out var flag))
        {
            return flag ? "Да" : "Нет";
        }

        if ((propertyName is "start_date" or "end_date") &&
            DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date.LocalDateTime.ToString("dd.MM.yyyy HH:mm");
        }

        return value;
    }
}
