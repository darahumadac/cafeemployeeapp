using CafeEmployeeApi.Contracts.Commands;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CafeEmployeeApi.Contracts;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(request)));
        if (validationResults.Any(v => !v.IsValid))
        {
            var errors = validationResults.Where(e => !e.IsValid).SelectMany(e => e.Errors).ToList();
            return (TResponse)Activator.CreateInstance(typeof(TResponse), new ValidationResult(errors))!;
        }

        return await next();
    }
}