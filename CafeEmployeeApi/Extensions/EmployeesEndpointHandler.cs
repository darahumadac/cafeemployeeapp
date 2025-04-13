using CafeEmployeeApi.Contracts;
using CafeEmployeeApi.Contracts.Commands;
using CafeEmployeeApi.Contracts.Queries;
using CafeEmployeeApi.Database;
using CafeEmployeeApi.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Extensions;

public static partial class EndpointExtensions
{
    private static async Task<IResult> GetEmployeesAsync(IMediator mediator, HttpContext context, [FromQuery] string? cafe = null)
    {
        var result = await mediator.Send(new GetEmployeesRequest(Cafe: cafe));
        if(!result.IsValid)
        {
            return Results.ValidationProblem(result.ValidationErrors!);
        }

        var employees = result.Value!;
        var response = employees
            .OrderByDescending(e => e.DaysWorked)
            .ThenBy(e => e.Name)
            .Select(e => new GetEmployeesResponse(
                Id: e.Id,
                Name: e.Name,
                EmailAddress: e.Email,
                PhoneNumber: e.PhoneNumber,
                DaysWorked: e.DaysWorked,
                Cafe: e.AssignedCafe?.Name ?? string.Empty
            ));

        context.Response.Headers.LastModified = employees.Count() > 0 ?
             employees.Max(c => c.UpdatedDate).ToString("R") 
             : DateTime.UtcNow.ToString("R");

        return Results.Ok(response);
    }

    private static async Task<IResult> GetEmployeeAsync(string id, AppDbContext dbContext, HttpContext context)
    {
        var employee = await dbContext.Employees.FindAsync(id);
        if (employee == null)
        {
            return Results.NotFound();
        }

        await dbContext.Entry(employee)
        .Reference(e => e.AssignedCafe)
        .LoadAsync();

        var response = new ViewEmployeeResponse(
            Id: employee.Id,
            Name: employee.Name,
            EmailAddress: employee.Email,
            PhoneNumber: employee.PhoneNumber,
            Gender: Convert.ToInt16(employee.Gender),
            DaysWorked: employee.DaysWorked,
            AssignedCafeId: employee.CafeId
        );

        context.Response.Headers.ETag = Convert.ToBase64String(employee.ETag);
        context.Response.Headers.LastModified = employee.UpdatedDate.ToString("R");

        return Results.Ok(response);

    }

    private static async Task<IResult> AddEmployeeAsync(IMediator mediator, EmployeeRequest request, IValidator<EmployeeRequest> validator, HttpContext context)
    {
        Result<CreateEmployeeResponse> result = await mediator.Send(request);
        if(!result.IsValid)
        {
            return Results.ValidationProblem(result.ValidationErrors!);
        }
        
        if(!result.IsSuccess)
        {
            return Results.Problem(detail: "The employee already exists", statusCode: 409);
        }
        
        var response = result.Value!;
        context.Response.Headers.ETag = response.ETag;

        return Results.CreatedAtRoute("GetEmployee", new { id = response.Id }, response);

    }

    private static async Task<IResult> UpdateEmployeeAsync(IMediator mediator, string id, EmployeeRequest request, HttpContext context)
    {

        var updateRequest = new UpdateEmployeeRequest(
            id, 
            request.Name, 
            request.EmailAddress, 
            request.PhoneNumber, 
            request.Gender,
            context.Request.Headers.IfMatch!, 
            request.AssignedCafeId);

        
        var result = await mediator.Send<Result<Employee>>(updateRequest);
        if(!result.IsValid)
        {
            return Results.ValidationProblem(result.ValidationErrors!);
        }

        if(!result.IsSuccess)
        {
            if(result.Error == StatusCodes.Status404NotFound.ToString())
            {
                return Results.NotFound();
            }
            else if(result.Error == StatusCodes.Status412PreconditionFailed.ToString())
            {
                return Results.StatusCode(StatusCodes.Status412PreconditionFailed);
            }

            return Results.Problem(detail: "The employee already exists", statusCode: 409);
        }

        var employee = result.Value!;
        context.Response.Headers.ETag = Convert.ToBase64String(employee.ETag);
        context.Response.Headers.LastModified = employee.UpdatedDate.ToString("R");
        
        return Results.Ok();

    }

    private static async Task<IResult> DeleteEmployeeAsync(IMediator mediator, string id, HttpContext context)
    {
        var ok = await mediator.Send(new DeleteEmployeeRequest(id));
        if(!ok)
        {
            return Results.NotFound();
        }
        context.Response.Headers.LastModified = DateTime.UtcNow.ToString("R");
        return Results.NoContent();
    }

}