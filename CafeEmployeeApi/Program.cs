using CafeEmployeeApi.Contracts.Commands;
using CafeEmployeeApi.Contracts.Queries;
using CafeEmployeeApi.Database;
using CafeEmployeeApi.Extensions;
using CafeEmployeeApi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Database
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb")));
//TODO: add logging

//TODO: Refactor repetitive code, do cqrs
//TODO: differentiate db exception with timeout exception for db

//Validation
builder.Services.AddRequestValidators();

//for exception handling
builder.Services.AddProblemDetails();

builder.Services.AddOutputCache(); //default expire: 60 sec

var app = builder.Build();

//Set exception handler
app.UseExceptionHandler(new ExceptionHandlerOptions
{
    AllowStatusCode404Response = true,
    StatusCodeSelector = ex =>
    {
        var statusCode = ex switch
        {
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
        return statusCode;
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//TODO: add cors here

app.UseOutputCache();

app.MapGet("/cafes", async (AppDbContext dbContext, HttpContext context, [FromQuery] string? location = null) =>
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

    context.Response.Headers.LastModified = cafes.Max(c => c.UpdatedDate).ToString("R");

    return Results.Ok(response);

}).WithName("GetCafes")
.CacheOutput();


app.MapGet("/employees", async (AppDbContext dbContext, HttpContext context, [FromQuery] string? cafe = null) =>
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

    context.Response.Headers.LastModified = employees.Max(c => c.UpdatedDate).ToString("R");

    return Results.Ok(response);

}).WithName("GetEmployees")
.CacheOutput();

app.MapGet("/cafes/{id}", async (string id, AppDbContext dbContext, HttpContext context) =>
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

}).WithName("GetCafe")
.CacheOutput();

app.MapGet("/employees/{id}", async (string id, AppDbContext dbContext, HttpContext context) =>
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

}).WithName("GetEmployee")
.CacheOutput();

app.MapPost("/cafe", async (CafeRequest request, AppDbContext dbContext, IValidator<CafeRequest> validator, HttpContext context) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    var newCafe = new Cafe
    {
        Name = request.Name,
        Description = request.Description,
        Location = request.Location,
        Logo = request.Logo
    };

    try
    {
        dbContext.Cafes.Add(newCafe);
        await dbContext.SaveChangesAsync();
        var response = new CreateCafeResponse(
            Id: newCafe.Id,
            Name: newCafe.Name,
            Description: newCafe.Description,
            Location: newCafe.Location,
            Logo: newCafe.Logo
        );

        context.Response.Headers.ETag = Convert.ToBase64String(newCafe.ETag);

        return Results.CreatedAtRoute("GetCafe", new { id = newCafe.Id.ToString() }, response);

    }
    catch (DbUpdateException ex)
    {
        //TODO: add logging
        return Results.Problem(detail: "The cafe already exists in the location", statusCode: 409);
    }



}).WithName("AddCafe");

app.MapPost("/employee", async (EmployeeRequest request, AppDbContext dbContext, IValidator<EmployeeRequest> validator, HttpContext context) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    var newEmployee = new Employee
    {
        Name = request.Name,
        Email = request.EmailAddress,
        PhoneNumber = request.PhoneNumber,
        Gender = Convert.ToBoolean(request.Gender),
        CafeId = request.AssignedCafeId != null ? Guid.Parse(request.AssignedCafeId) : null,
        StartDate = request.AssignedCafeId != null ? DateTime.UtcNow : null
    };

    try
    {
        dbContext.Employees.Add(newEmployee);

        await dbContext.SaveChangesAsync();

        var response = new CreateEmployeeResponse(
            Id: newEmployee.Id,
            Name: newEmployee.Name,
            Email: newEmployee.Email,
            PhoneNumber: newEmployee.PhoneNumber,
            Gender: Convert.ToInt16(newEmployee.Gender),
            CafeId: newEmployee.CafeId
        );

        context.Response.Headers.ETag = Convert.ToBase64String(newEmployee.ETag);

        return Results.CreatedAtRoute("GetEmployee", new { id = newEmployee.Id }, response);
    }
    catch (DbUpdateException ex)
    {
        return Results.Problem(detail: "The employee already exists", statusCode: 409);
    }

}).WithName("AddEmployee");

app.MapPut("/cafe/{id}", async (string id, CafeRequest request, AppDbContext dbContext, IValidator<CafeRequest> validator, HttpContext context) =>
{
    if (string.IsNullOrEmpty(context.Request.Headers.IfMatch))
    {
        return Results.Problem(detail: "Missing If-Match header.", statusCode: StatusCodes.Status428PreconditionRequired);
    }

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

}).WithName("UpdateCafe");

app.MapPut("/employee/{id}", async (string id, EmployeeRequest request, AppDbContext dbContext, IValidator<EmployeeRequest> validator, HttpContext context) =>
{
    if (string.IsNullOrEmpty(context.Request.Headers.IfMatch))
    {
        return Results.Problem(detail: "Missing If-Match header.", statusCode: StatusCodes.Status428PreconditionRequired);
    }

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



}).WithName("UpdateEmployee");


app.MapDelete("/cafe/{id}", async (string id, AppDbContext dbContext) =>
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

    await dbContext.Entry(cafe).Collection(c => c.Employees).LoadAsync();
    foreach (var employee in cafe.Employees)
    {
        employee.StartDate = null;
    }

    dbContext.Cafes.Remove(cafe);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();

}).WithName("DeleteCafe");

app.MapDelete("/employee/{id}", async (string id, AppDbContext dbContext) =>
{
    var employee = await dbContext.Employees.FindAsync(id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    dbContext.Employees.Remove(employee);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();

}).WithName("DeleteEmployee");


app.Run();
