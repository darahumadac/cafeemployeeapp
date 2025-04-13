using CafeEmployeeApi.Contracts.Commands;
using CafeEmployeeApi.Database;
using CafeEmployeeApi.Extensions;
using CafeEmployeeApi.Models;
using CafeEmployeeApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Database
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb")));
//TODO: add logging

builder.Services.AddCrudServices();
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

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

//TODO: group api versions here

//map endpoints
app.MapCafeEndpoints();
app.MapEmployeeEndpoints();

app.Run();
