using CafeEmployeeApi.Contracts;
using CafeEmployeeApi.Contracts.Commands;
using CafeEmployeeApi.Contracts.Queries;
using CafeEmployeeApi.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Database
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb")));

var app = builder.Build();

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
    if(!string.IsNullOrEmpty(cafe))
    {
        var validGuid = Guid.TryParse(cafe, out cafeId);
        if(!validGuid)
        {
            return Results.NotFound("Cafe not found");
        }
    }
    
    var employees = await dbContext.Employees
        .Where(e => string.IsNullOrEmpty(cafe) || e.CafeId == cafeId)
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
     Guid cafeId = Guid.Empty;
    if(!string.IsNullOrEmpty(id))
    {
        var validGuid = Guid.TryParse(id, out cafeId);
        if(!validGuid)
        {
            return Results.NotFound();
        }
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
        Gender: employee.Gender,
        AssignedCafeId: employee.CafeId
    );

    return Results.Ok(response);

}).WithName("GetEmployee");

app.MapPost("/cafe", (AppDbContext dbContext) =>
{

}).WithName("AddCafe");

app.MapPost("/employees", (AppDbContext dbContext) =>
{
    //TODO: Add employee to cafe. no employee can be employed by multiple cafes within the same employment period 
}).WithName("AddEmployee");

app.MapPut("/cafe/{id}", (Guid id, AppDbContext dbContext) =>
{

}).WithName("UpdateCafe");

app.MapPut("/employees/{id}", (string id, AppDbContext dbContext) =>
{

}).WithName("UpdateEmployee");

app.MapDelete("/cafe/{id}", (Guid id, AppDbContext dbContext) =>
{

}).WithName("DeleteCafe");

app.MapDelete("/employees/{id}", (string id, AppDbContext dbContext) =>
{

}).WithName("DeleteEmployee");


app.Run();
