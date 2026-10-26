using TodoListApp.Core.Entities;

namespace TodoListApp.Core.Dtos;

public static class JobItemCustomMapExtensions
{
    public static CreateJobDto ToCreateJobDto(this JobItemEntity entity)
    {
        return new CreateJobDto
        {
            EmployeerId = entity.employeer_id,
            Title = entity.title,
            Description = entity.description ?? string.Empty,
            Website = entity.website ?? string.Empty,
            Price = entity.price,
            StartDate = entity.start_date,
        };
    }

    public static UpdateJobDto ToUpdateJobDto(this JobItemEntity entity)
    {
        return new UpdateJobDto
        {
            Id = entity.id,
            EmployeerId = entity.employeer_id,
            Title = entity.title,
            Description = entity.description ?? string.Empty,
            Website = entity.website ?? string.Empty,
            Price = entity.price,
            IsCompleted = entity.is_completed,
            IsPayed = entity.is_payed,
            StartDate = entity.start_date,
            EndDate = entity.end_date.HasValue ? DateOnly.FromDateTime(entity.end_date.Value.DateTime) : null
        };
    }

    public static JobItemEntity ToEntity(this CreateJobDto dto)
    {
        return new JobItemEntity
        {
            employeer_id = dto.EmployeerId,
            title = dto.Title,
            description = dto.Description,
            website = dto.Website,
            price = dto.Price,
            start_date = dto.StartDate,
        };
    }

    public static JobItemEntity ToEntity(this UpdateJobDto dto)
    {
        return new JobItemEntity
        {
            id = dto.Id,
            employeer_id = dto.EmployeerId,
            title = dto.Title,
            description = dto.Description,
            website = dto.Website,
            price = dto.Price,
            is_completed = dto.IsCompleted,
            is_payed = dto.IsPayed,
            start_date = dto.StartDate,
            end_date = dto.EndDate.HasValue ? new DateTimeOffset(dto.EndDate.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero) : null
        };
    }
}