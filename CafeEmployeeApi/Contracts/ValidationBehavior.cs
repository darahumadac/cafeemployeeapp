using FluentValidation;
using MediatR;

namespace CafeEmployeeApi.Contracts;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull 
{
    private readonly IValidator<TRequest> _validator;

    public ValidationBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
         var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return (TResponse)Activator.CreateInstance(typeof(TResponse), validationResult)!;
        }

        return await next();
    }
}