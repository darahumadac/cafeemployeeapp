using FluentValidation;

namespace CafeEmployeeApi.Contracts.Commands;

public record CreateCafeRequest(
    string Name, 
    string Description, 
    string Location, 
    byte[]? Logo = null);

public class CreateCafeRequestValidator : AbstractValidator<CreateCafeRequest>
{
    public CreateCafeRequestValidator()
    {
        RuleFor(r => r.Name).NotNull().NotEmpty().Length(6, 10);
        RuleFor(r => r.Description).NotNull().NotEmpty().MaximumLength(256);
        //TODO: Add logo validation
        RuleFor(r => r.Location).NotNull().NotEmpty();
    }
}