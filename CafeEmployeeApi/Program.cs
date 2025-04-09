using CafeEmployeeApi.Contracts;
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
            return Results.Ok(new List<EmployeesResponse>());
        }
    }
    
    var employees = await dbContext.Employees
        .Where(e => string.IsNullOrEmpty(cafe) || e.CafeId == cafeId)
        .Include(e => e.AssignedCafe)
        .ToListAsync();

    var response = employees
        .OrderByDescending(e => e.DaysWorked)
        .ThenBy(e => e.Name)
        .Select(e => new EmployeesResponse(
            Id: e.Id,
            Name: e.Name,
            EmailAddress: e.Email,
            PhoneNumber: e.PhoneNumber,
            DaysWorked: e.DaysWorked,
            Cafe: e.AssignedCafe?.Name ?? string.Empty
        ));

    return Results.Ok(response);

}).WithName("GetEmployees");

app.MapGet("/cafes/{id}", (Guid id) =>
{

}).WithName("GetCafe");

app.MapGet("/employees/{id}", (string id) =>
{

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
