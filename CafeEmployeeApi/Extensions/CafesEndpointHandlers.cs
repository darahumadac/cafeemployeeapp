using CafeEmployeeApi.Contracts;
using CafeEmployeeApi.Contracts.Commands;
using CafeEmployeeApi.Contracts.Queries;
using CafeEmployeeApi.Database;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CafeEmployeeApi.Extensions;

public static partial class EndpointExtensions
{
    private static async Task<IResult> GetCafesAsync(IMediator mediator, HttpContext context, [FromQuery] string? location = null)
    {
        var cafes = await mediator.Send(new GetCafesRequest(location));
        var response = cafes.Select(c => new GetCafesResponse(
            Name: c.Name,
            Description: c.Description,
            Employees: c.Employees.Count,
            Location: c.Location,
            Id: c.Id,
            Logo: c.Logo
        ));
    
        context.Response.Headers.LastModified = cafes.Count() > 0 ?
             cafes.Max(c => c.UpdatedDate).ToString("R") 
             : DateTime.UtcNow.ToString("R");

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

    private static async Task<IResult> AddCafeAsync(IMediator mediator, CreateCafeRequest request, HttpContext context)
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
        context.Response.Headers.LastModified = DateTime.UtcNow.ToString("R");

        return Results.CreatedAtRoute("GetCafe", new { id = response.Id.ToString() }, response);
    }

    private static async Task<IResult> UpdateCafeAsync(IMediator mediator, string id, CreateCafeRequest request, HttpContext context)
    {
        var updateRequest = new UpdateCafeRequest(
            id, 
            request.Name, 
            request.Description, 
            request.Location, 
            context.Request.Headers.IfMatch!, 
            request.Logo);

        var result = await mediator.Send(updateRequest);
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

            return Results.Problem(detail: "The cafe already exists in the location", statusCode: 409);
        }

        var cafe = result.Value!;
        context.Response.Headers.ETag = Convert.ToBase64String(cafe.ETag);
        context.Response.Headers.LastModified = cafe.UpdatedDate.ToString("R");

        return Results.Ok();

    }

    private static async Task<IResult> DeleteCafeAsync(IMediator mediator, string id, HttpContext context)
    {
        var result = await mediator.Send(new DeleteCafeRequest(id));
        if(!result.IsValid)
        {
            return Results.ValidationProblem(result.ValidationErrors!);
        }
        
        if(!result.Value)
        {
            return Results.NotFound();
        }

        context.Response.Headers.LastModified = DateTime.UtcNow.ToString("R");
      
        return Results.NoContent();
    }
}