using TodoListApp.Core.Entities;

namespace TodoListApp.Core.Dtos;

public static class EmployeerCustomMapExtensions
{
    public static CreateEmployeerDto ToCreateEmployeerDto(this EmployeerEntity entity)
    {
        return new CreateEmployeerDto
        {
            Name = entity.name,
        };
    }
    public static EmployeerEntity ToEntity(this CreateEmployeerDto dto)
    {
        return new EmployeerEntity
        {
            name = dto.Name,
        };
    }
}