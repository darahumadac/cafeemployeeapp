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
    private static async Task<IResult> GetEmployeesAsync(AppDbContext dbContext, HttpContext context, [FromQuery] string? cafe = null)
    {
        Guid cafeId = Guid.Empty;
        var validGuid = cafe != null && cafe != string.Empty ? cafe.TryToGuid(out cafeId) : true;
        if (!validGuid)
        {
            return Results.NotFound();
        }

        var employees = await dbContext.Employees
            .Where(e => cafeId == Guid.Empty || e.CafeId == cafeId)
            .Include(e => e.AssignedCafe)
            .ToListAsync();

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

        context.Response.Headers.LastModified = employees.Count > 0 ? employees.Max(c => c.UpdatedDate).ToString("R") : DateTime.UtcNow.ToString("R");

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
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        Result<CreateEmployeeResponse> result = await mediator.Send(request);
        if(!result.IsSuccess)
        {
            return Results.Problem(detail: "The employee already exists", statusCode: 409);
        }
        
        var response = result.Value!;
        context.Response.Headers.ETag = response.ETag;

        return Results.CreatedAtRoute("GetEmployee", new { id = response.Id }, response);

    }

    private static async Task<IResult> UpdateEmployeeAsync(string id, EmployeeRequest request, AppDbContext dbContext, IValidator<EmployeeRequest> validator, HttpContext context)
    {
        var employee = await dbContext.Employees.FindAsync(id);
        if (employee == null)
        {
            return Results.NotFound();
        }

        if (Convert.ToBase64String(employee.ETag) != context.Request.Headers.IfMatch)
        {
            return Results.StatusCode(StatusCodes.Status412PreconditionFailed);
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        Guid? newCafeId = request.AssignedCafeId != null ? Guid.Parse(request.AssignedCafeId) : null;
        var currentCafeId = employee.CafeId;

        //update start date only when changing assigned cafe
        var now = DateTime.UtcNow;
        if (newCafeId != currentCafeId)
        {
            employee.StartDate = newCafeId != null ? now : null;
        }

        employee.Name = request.Name;
        employee.Email = request.EmailAddress;
        employee.PhoneNumber = request.PhoneNumber;
        employee.Gender = Convert.ToBoolean(request.Gender);
        employee.CafeId = newCafeId;
        employee.UpdatedDate = now;

        try
        {
            await dbContext.SaveChangesAsync();
            context.Response.Headers.ETag = Convert.ToBase64String(employee.ETag);
            context.Response.Headers.LastModified = employee.UpdatedDate.ToString("R");
            return Results.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Results.Problem(detail: "The employee already exists", statusCode: 409);
        }

    }

    private static async Task<IResult> DeleteEmployeeAsync(IMediator mediator, string id)
    {
        var ok = await mediator.Send(new DeleteEmployeeRequest(id));
        if(!ok)
        {
            return Results.NotFound();
        }
        return Results.NoContent();
    }

}