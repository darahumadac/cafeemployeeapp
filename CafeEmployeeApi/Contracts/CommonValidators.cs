using FluentValidation;

namespace CafeEmployeeApi.Contracts;

public class NoValidation<TRequest> : AbstractValidator<TRequest>
{
    public NoValidation(){}
}

public record GuidRequest(string? Guid);
public class GuidValidator : AbstractValidator<GuidRequest>
{
    public GuidValidator()
    {
        When(r => !string.IsNullOrEmpty(r.Guid), () => 
        {
            RuleFor(r => r.Guid)
            .Must(cafeId => Guid.TryParse(cafeId, out Guid parsedCafeId))
            .WithMessage("Invalid Cafe ID format");
        });
    }
}
