using FluentValidation;

namespace CafeEmployeeApi.Contracts.Commands;

public record CafeRequest(
    string Name, 
    string Description, 
    string Location, 
    byte[]? Logo = null);

public class CafeRequestValidator : AbstractValidator<CafeRequest>
{
    public CafeRequestValidator()
    {
        RuleFor(r => r.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(6, 10);

        RuleFor(r => r.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(256);

        //TODO: Add logo validation
        
        RuleFor(r => r.Location).NotEmpty();
    }
}