using CafeEmployeeApi.Contracts.Commands;
using FluentValidation;

namespace CafeEmployeeApi.Extensions;

public static class ValidationExtensions
{
    public static void AddRequestValidators(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IValidator<CafeRequest>, CafeRequestValidator>();
        builder.Services.AddScoped<IValidator<EmployeeRequest>, EmployeeRequestValidator>();
    }
}