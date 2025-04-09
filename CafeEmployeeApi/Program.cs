using CafeEmployeeApi.Database;
using CafeEmployeeApi.Models;
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

app.MapGet("/cafes", ([FromQuery] string location) => {
    
}).WithName("GetCafes");

app.MapGet("/employees", ([FromQuery] string cafe) => {

}).WithName("GetEmployees");

app.MapPost("/cafe", (AppDbContext dbContext) => {    

}).WithName("AddCafe");

app.MapPost("/employees", (AppDbContext dbContext) => {    
    //TODO: Add employee to cafe. no employee can be employed by multiple cafes within the same employment period 
}).WithName("AddEmployee");

app.MapPut("/cafe/{cafeId}", (Guid cafeId, AppDbContext dbContext) => {    

}).WithName("UpdateCafe");

app.MapPut("/employees/{employeeId}", (string employeeId, AppDbContext dbContext) => {    

}).WithName("UpdateEmployee");

app.MapDelete("/cafe/{cafeId}", (Guid cafeId, AppDbContext dbContext) => {    

}).WithName("DeleteCafe");

app.MapDelete("/employees/{employeeId}", (string employeeId, AppDbContext dbContext) => {    

}).WithName("DeleteEmployee");


app.Run();
