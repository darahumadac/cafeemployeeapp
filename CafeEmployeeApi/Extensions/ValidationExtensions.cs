using CafeEmployeeApi.Contracts.Commands;
using FluentValidation;

namespace CafeEmployeeApi.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddRequestValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CafeRequest>, CafeRequestValidator>();
        services.AddScoped<IValidator<EmployeeRequest>, EmployeeRequestValidator>();
        
        return services;
    }
}