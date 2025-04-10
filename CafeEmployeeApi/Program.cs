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

//TODO: add extension method for validators
//TODO: add logging
//Validation
builder.Services.AddScoped<IValidator<CreateCafeRequest>, CreateCafeRequestValidator>();
builder.Services.AddScoped<IValidator<UpsertEmployeeRequest>, UpsertEmployeeRequestValidator>();

//for exception handling
builder.Services.AddProblemDetails();

var app = builder.Build();

//Set exception handler
app.UseExceptionHandler(new ExceptionHandlerOptions
{
    AllowStatusCode404Response = true,
    StatusCodeSelector = ex => ex is BadHttpRequestException
        ? StatusCodes.Status400BadRequest
        : StatusCodes.Status500InternalServerError
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/cafes", async (AppDbContext dbContext, [FromQuery] string? location = null) =>
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

    return Results.Ok(response);

}).WithName("GetCafes");


app.MapGet("/employees", async (AppDbContext dbContext, [FromQuery] string? cafe = null) =>
{
    Guid cafeId = Guid.Empty;
    var validGuid = cafe != null && cafe != string.Empty ? cafe.ToGuid(out cafeId) : true;
    if(!validGuid)
    {
        return Results.NotFound("Cafe not found");
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

    return Results.Ok(response);

}).WithName("GetEmployees");

app.MapGet("/cafes/{id}", async (string id, AppDbContext dbContext) =>
{
    var validGuid = id.ToGuid(out Guid cafeId);
    if(!validGuid)
    {
        return Results.NotFound();
    }
    
    var cafe = await dbContext.Cafes.FindAsync(cafeId);
    if(cafe == null)
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

    return Results.Ok(response);
    
}).WithName("GetCafe");

app.MapGet("/employees/{id}", async (string id, AppDbContext dbContext) =>
{
    var employee = await dbContext.Employees.FindAsync(id);
    if(employee == null)
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
        AssignedCafeId: employee.CafeId
    );

    return Results.Ok(response);

}).WithName("GetEmployee");

app.MapPost("/cafe", async (CreateCafeRequest request, AppDbContext dbContext, IValidator<CreateCafeRequest> validator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if(!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    
    var newCafe = new Cafe{
        Name = request.Name,
        Description = request.Description,
        Location = request.Location,
        Logo = request.Logo
    };

    dbContext.Cafes.Add(newCafe);

    await dbContext.SaveChangesAsync();

    return Results.CreatedAtRoute("GetCafe", new {id = newCafe.Id.ToString()}, newCafe);

}).WithName("AddCafe");

app.MapPost("/employee", async (UpsertEmployeeRequest request, AppDbContext dbContext, IValidator<UpsertEmployeeRequest> validator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if(!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    var newEmployee = new Employee{
        Name = request.Name,
        Email = request.EmailAddress,
        PhoneNumber = request.PhoneNumber,
        Gender = Convert.ToBoolean(request.Gender),
        CafeId = request.AssignedCafeId != null ? Guid.Parse(request.AssignedCafeId) : null,
    };

    dbContext.Employees.Add(newEmployee);
    
    
    await dbContext.SaveChangesAsync();

    return Results.CreatedAtRoute("GetEmployee", new {id = newEmployee.Id}, new {
        id = newEmployee.Id,
        name = newEmployee.Name,
        email = newEmployee.Email,
        phoneNumber = newEmployee.PhoneNumber,
        gender = Convert.ToInt16(newEmployee.Gender),
        cafeId = newEmployee.CafeId
    });


}).WithName("AddEmployee");

app.MapPut("/cafe/{id}", (string id, AppDbContext dbContext) =>
{

}).WithName("UpdateCafe");

app.MapPut("/employee/{id}", async (string id, UpsertEmployeeRequest request, AppDbContext dbContext, IValidator<UpsertEmployeeRequest> validator) =>
{
    var employee = await dbContext.Employees.FindAsync(id);
    if(employee == null)
    {
        return Results.NotFound();
    }

    var validationResult = await validator.ValidateAsync(request);
    if(!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    employee.Name = request.Name;
    employee.Email = request.EmailAddress;
    employee.PhoneNumber = request.PhoneNumber;
    employee.Gender = Convert.ToBoolean(request.Gender);
    employee.CafeId = request.AssignedCafeId != null ? Guid.Parse(request.AssignedCafeId) : null;

    await dbContext.SaveChangesAsync();

    return Results.Ok();

}).WithName("UpdateEmployee");


app.MapDelete("/cafe/{id}", async (string id, AppDbContext dbContext) =>
{   
    var validGuid = id.ToGuid(out Guid cafeId);
    if(!validGuid)
    {
        return Results.NotFound();
    }

    var cafe = await dbContext.Cafes.FindAsync(cafeId);
    if(cafe == null)
    {
        return Results.NotFound();
    }

    dbContext.Cafes.Remove(cafe);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();

}).WithName("DeleteCafe");

app.MapDelete("/employee/{id}", async (string id, AppDbContext dbContext) =>
{
    var employee = await dbContext.Employees.FindAsync(id);
    if(employee == null)
    {
        return Results.NotFound();
    }
    dbContext.Employees.Remove(employee);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();

}).WithName("DeleteEmployee");


app.Run();
