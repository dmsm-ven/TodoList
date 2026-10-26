using FluentValidation;
using TodoListApp.Core.Dtos;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.Validators;

public class CreateEmployeerDtoValidator : AbstractValidator<CreateEmployeerDto>
{
    public CreateEmployeerDtoValidator(IEmployeerRepository repo)
    {
        RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Name is required.")
               .MaximumLength(256).WithMessage("Name must not exceed 256 characters.")
               .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
               .MustAsync(async (currentName, cancellation) =>
               {
                   var all = await repo.GetAllEmployeer();
                   return all.Count(emp => emp.name == currentName) == 0;
               })
            .WithMessage("An employeer with the same name already exists.");
    }
}
