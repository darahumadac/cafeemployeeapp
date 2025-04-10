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

//TODO: trim all strings, html encode strings, normalize strings
//TODO: add created date and updated date for records
//TODO: add etag and last modified headers for create and update request

//Validation
builder.Services.AddScoped<IValidator<CafeRequest>, CafeRequestValidator>();
builder.Services.AddScoped<IValidator<EmployeeRequest>, EmployeeRequestValidator>();

//for exception handling
builder.Services.AddProblemDetails();

var app = builder.Build();

//Set exception handler
app.UseExceptionHandler(new ExceptionHandlerOptions
{
    AllowStatusCode404Response = true,
    StatusCodeSelector = ex => {
        var statusCode =  ex switch
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
    var validGuid = cafe != null && cafe != string.Empty ? cafe.TryToGuid(out cafeId) : true;
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
    var validGuid = id.TryToGuid(out Guid cafeId);
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
        DaysWorked: employee.DaysWorked,
        AssignedCafeId: employee.CafeId
    );

    return Results.Ok(response);

}).WithName("GetEmployee");

app.MapPost("/cafe", async (CafeRequest request, AppDbContext dbContext, IValidator<CafeRequest> validator) =>
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

    try
    {
        dbContext.Cafes.Add(newCafe);
        await dbContext.SaveChangesAsync();
        return Results.CreatedAtRoute("GetCafe", new {id = newCafe.Id.ToString()}, newCafe);

    }catch(DbUpdateException ex)
    {
        //TODO: add logging
        return Results.Problem(detail: "The cafe already exists in the location", statusCode: 409);
    }

    

}).WithName("AddCafe");

app.MapPost("/employee", async (EmployeeRequest request, AppDbContext dbContext, IValidator<EmployeeRequest> validator) =>
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
        StartDate = request.AssignedCafeId != null ? DateTime.UtcNow : null
    };

    try
    {
        dbContext.Employees.Add(newEmployee);

        await dbContext.SaveChangesAsync();

        return Results.CreatedAtRoute("GetEmployee", new {id = newEmployee.Id}, new {
            id = newEmployee.Id,
            name = newEmployee.Name,
            email = newEmployee.Email,
            phoneNumber = newEmployee.PhoneNumber,
            gender = Convert.ToInt16(newEmployee.Gender),
            cafeId = newEmployee.CafeId,
            daysWorked = 0
        });
    }
    catch(DbUpdateException ex)
    {
        return Results.Problem(detail: "The employee already exists", statusCode: 409);
    }
    


}).WithName("AddEmployee");

app.MapPut("/cafe/{id}", async (string id, CafeRequest request, AppDbContext dbContext, IValidator<CafeRequest> validator) =>
{
    var validGuid = id.TryToGuid(out Guid cafeId);
    if(!validGuid)
    {
        return Results.NotFound();
    }

    var cafe = await dbContext.Cafes.FindAsync(cafeId);
    if(cafe == null)
    {
        return Results.NotFound();
    }
    
    var validationResult = await validator.ValidateAsync(request);
    if(!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    cafe.Name = request.Name;
    cafe.Description = request.Description;
    cafe.Location = request.Location;
    cafe.Logo = request.Logo;

    try
    {
        await dbContext.SaveChangesAsync();
        return Results.Ok();

    }catch(DbUpdateException ex)
    {
        return Results.Problem(detail: "The cafe already exists in the location", statusCode: 409);
    }

}).WithName("UpdateCafe");

app.MapPut("/employee/{id}", async (string id, EmployeeRequest request, AppDbContext dbContext, IValidator<EmployeeRequest> validator) =>
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

    Guid? newCafeId = request.AssignedCafeId != null ? Guid.Parse(request.AssignedCafeId) : null;
    var currentCafeId = employee.CafeId;

    //update start date only when changing assigned cafe
    if(newCafeId != currentCafeId)
    {
        employee.StartDate = newCafeId != null ? DateTime.UtcNow :  null;
    }
    
    employee.Name = request.Name;
    employee.Email = request.EmailAddress;
    employee.PhoneNumber = request.PhoneNumber;
    employee.Gender = Convert.ToBoolean(request.Gender);
    employee.CafeId = newCafeId;

    try
    {
        await dbContext.SaveChangesAsync();
        return Results.Ok();
    }catch(DbUpdateException ex)
    {
        return Results.Problem(detail: "The employee already exists", statusCode: 409);
    }

    

}).WithName("UpdateEmployee");


app.MapDelete("/cafe/{id}", async (string id, AppDbContext dbContext) =>
{   
    var validGuid = id.TryToGuid(out Guid cafeId);
    if(!validGuid)
    {
        return Results.NotFound();
    }

    var cafe = await dbContext.Cafes.FindAsync(cafeId);
    if(cafe == null)
    {
        return Results.NotFound();
    }

    await dbContext.Entry(cafe).Collection(c => c.Employees).LoadAsync();
    foreach(var employee in cafe.Employees)
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
    if(employee == null)
    {
        return Results.NotFound();
    }
    dbContext.Employees.Remove(employee);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();

}).WithName("DeleteEmployee");


app.Run();
