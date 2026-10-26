using FluentValidation;
using TodoListApp.Core.Dtos;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.Validators;

public class CreateJobDtoValidator : AbstractValidator<CreateJobDto>
{
    public CreateJobDtoValidator(IEmployeerRepository empRepo)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(256).WithMessage("Title must not exceed 256 characters.");

        RuleFor(x => x.EmployeerId)
            .GreaterThan(0).WithMessage("EmployeerId must be selected");

        RuleFor(x => x.EmployeerId)
            .MustAsync(async (employeerId, cancellation) =>
            {
                var emp = await empRepo.GetEmployeer(employeerId);
                return emp != null;
            })
            .WithMessage("Employeer does not exist.");
    }
}

public class UpdateJobDtoValidator : AbstractValidator<UpdateJobDto>
{
    public UpdateJobDtoValidator(IEmployeerRepository empRepo)
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id does not exist");
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(256).WithMessage("Title must not exceed 256 characters.");
        RuleFor(x => x.EmployeerId)
            .GreaterThan(0).WithMessage("EmployeerId must be selected");
        RuleFor(x => x.EmployeerId)
            .MustAsync(async (employeerId, cancellation) =>
            {
                var emp = await empRepo.GetEmployeer(employeerId);
                return emp != null;
            })
            .WithMessage("Employeer does not exist.");
    }
}