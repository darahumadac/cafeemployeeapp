using CafeEmployeeApi.Database;
using FluentValidation;

namespace CafeEmployeeApi.Contracts.Commands;

public record EmployeeRequest(
    string Name, 
    string EmailAddress, 
    string PhoneNumber, 
    int Gender, 
    string? AssignedCafeId);

public class EmployeeRequestValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidator(AppDbContext dbContext)
    {
        RuleFor(r => r.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(6, 10);

        RuleFor(r => r.EmailAddress)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(r => r.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(number => number.Length == 8 && (number.StartsWith("8") || number.StartsWith("9")))
                .WithMessage("'Phone Number' is not a valid SG phone number");
        
        
        RuleFor(r => r.Gender)
            .InclusiveBetween(0,1);
            

        When(r => r.AssignedCafeId != null, () => {
            RuleFor(r => r.AssignedCafeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("'AssignedCafeId' must not be empty. Remove 'AssignedCafeId' from the request to remove assigned cafe")
                .Must(cafeId => 
                    Guid.TryParse(cafeId, out Guid parsedCafeId) 
                    && dbContext.Cafes.Find(parsedCafeId) != null)
                    .WithMessage("'AssignedCafeId' must be an existing cafe id");
        });
    }
}