using CafeEmployeeApi.Contracts;
using CafeEmployeeApi.Contracts.Commands;
using CafeEmployeeApi.Models;
using CafeEmployeeApi.Services;
using FluentValidation;
using MediatR;

namespace CafeEmployeeApi.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddRequestValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CafeRequest>, CafeRequestValidator>();
        services.AddScoped<IValidator<EmployeeRequest>, EmployeeRequestValidator>();
        services.AddScoped<IValidator<DeleteCafeRequest>, NoValidation<DeleteCafeRequest>>();
        services.AddScoped<IValidator<DeleteEmployeeRequest>, NoValidation<DeleteEmployeeRequest>>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }

    public static IServiceCollection AddCrudServices(this IServiceCollection services)
    {
        services.AddScoped<IDeleteService<string>, EmployeeDeleteService>();
        services.AddScoped<IDeleteService<Guid>, CafeDeleteService>();
        services.AddScoped<IAddService<Cafe, CreateCafeResponse>, AddService<Cafe, CreateCafeResponse>>();
        services.AddScoped<IAddService<Employee, CreateEmployeeResponse>, AddService<Employee, CreateEmployeeResponse>>();

        return services;
    } 
}