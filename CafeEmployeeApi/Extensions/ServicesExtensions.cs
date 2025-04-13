using CafeEmployeeApi.Contracts;
using CafeEmployeeApi.Contracts.Commands;
using CafeEmployeeApi.Contracts.Queries;
using CafeEmployeeApi.Models;
using CafeEmployeeApi.Services;
using FluentValidation;
using MediatR;

namespace CafeEmployeeApi.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddRequestValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateCafeRequest>, CafeRequestValidator>();
        services.AddScoped<IValidator<EmployeeRequest>, EmployeeRequestValidator>();
        services.AddScoped<IValidator<DeleteCafeRequest>, GuidValidator>();
        services.AddScoped<IValidator<DeleteEmployeeRequest>, NoValidation<DeleteEmployeeRequest>>();
        services.AddScoped<IValidator<GetCafesRequest>, NoValidation<GetCafesRequest>>();
        services.AddScoped<IValidator<GetEmployeesRequest>, GuidValidator>();
        services.AddScoped<IValidator<UpdateCafeRequest>, UpdateCafeIdValidator>();
        services.AddScoped<IValidator<UpdateCafeRequest>, CafeRequestValidator>();

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