using CafeEmployeeApi.Contracts;
using CafeEmployeeApi.Contracts.Commands;
using CafeEmployeeApi.Contracts.Queries;
using CafeEmployeeApi.Database;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Extensions;

public static partial class EndpointExtensions
{
    private static async Task<IResult> GetCafesAsync(AppDbContext dbContext, HttpContext context, [FromQuery] string? location = null)
    {
        //TODO: implement mediator
        // var result = await mediator.Send(new GetCafes(location));
        var cafes = await dbContext.Cafes
            .Where(c => string.IsNullOrEmpty(location) || c.Location == location)
            .Include(c => c.Employees)
            .OrderByDescending(c => c.Employees.Count)
            .ThenBy(c => c.Name)
            .ToListAsync();

        var response = cafes.Select(c => new GetCafesResponse(
            Name: c.Name,
            Description: c.Description,
            Employees: c.Employees.Count,
            Location: c.Location,
            Id: c.Id,
            Logo: c.Logo
        ));

        context.Response.Headers.LastModified = cafes.Count > 0 ? cafes.Max(c => c.UpdatedDate).ToString("R") : DateTime.UtcNow.ToString("R");

        return Results.Ok(response);
    }

    private static async Task<IResult> GetCafeAsync(string id, AppDbContext dbContext, HttpContext context)
    {
        var validGuid = id.TryToGuid(out Guid cafeId);
        if (!validGuid)
        {
            return Results.NotFound();
        }

        var cafe = await dbContext.Cafes.FindAsync(cafeId);
        if (cafe == null)
        {
            return Results.NotFound();
        }
        var response = new ViewCafeResponse(
            Name: cafe.Name,
            Description: cafe.Description,
            Location: cafe.Location,
            Id: cafe.Id,
            Logo: cafe.Logo
        );
        context.Response.Headers.ETag = Convert.ToBase64String(cafe.ETag);
        context.Response.Headers.LastModified = cafe.UpdatedDate.ToString("R");
        return Results.Ok(response);
    }

    private static async Task<IResult> AddCafeAsync(IMediator mediator, CafeRequest request, IValidator<CafeRequest> validator, HttpContext context)
    {
        Result<CreateCafeResponse> result = await mediator.Send(request);
        if(!result.IsValid)
        {
            return Results.ValidationProblem(result.ValidationErrors!);
        }
        
        if(!result.IsSuccess)
        {
            return Results.Problem(detail: "The cafe already exists in the location", statusCode: 409);
        }

        var response = result.Value!;
        context.Response.Headers.ETag = response.ETag;

        return Results.CreatedAtRoute("GetCafe", new { id = response.Id.ToString() }, response);
    }

    private static async Task<IResult> UpdateCafeAsync(string id, CafeRequest request, AppDbContext dbContext, IValidator<CafeRequest> validator, HttpContext context)
    {
        var validGuid = id.TryToGuid(out Guid cafeId);
        if (!validGuid)
        {
            return Results.NotFound();
        }

        var cafe = await dbContext.Cafes.FindAsync(cafeId);
        if (cafe == null)
        {
            return Results.NotFound();
        }

        if (Convert.ToBase64String(cafe.ETag) != context.Request.Headers.IfMatch)
        {
            return Results.StatusCode(StatusCodes.Status412PreconditionFailed);
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        cafe.Name = request.Name;
        cafe.Description = request.Description;
        cafe.Location = request.Location;
        cafe.Logo = request.Logo;
        cafe.UpdatedDate = DateTime.UtcNow;
        try
        {
            await dbContext.SaveChangesAsync();
            context.Response.Headers.ETag = Convert.ToBase64String(cafe.ETag);
            context.Response.Headers.LastModified = cafe.UpdatedDate.ToString("R");
            return Results.Ok();

        }
        catch (DbUpdateException ex)
        {
            return Results.Problem(detail: "The cafe already exists in the location", statusCode: 409);
        }

    }

    private static async Task<IResult> DeleteCafeAsync(IMediator mediator, string id)
    {
        var ok = await mediator.Send(new DeleteCafeRequest(id));
        if(!ok)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}