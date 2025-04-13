using CafeEmployeeApi.Database;
using CafeEmployeeApi.Extensions;
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
            .WithMethods("POST", "GET", "PUT", "DELETE")
            .WithHeaders("Content-Type", "Accept", "If-Match", "ETag", "If-Modified-Since");
    });
});

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
app.UseCors();

app.UseOutputCache();

//TODO: group api versions here

//middleware to check if-match header for put requests
app.Use(async (context, next) => {
    if(context.Request.Method == "PUT" && string.IsNullOrEmpty(context.Request.Headers.IfMatch))
    {
        await Results.Problem(
            detail: "Missing If-Match header.", 
            statusCode: StatusCodes.Status428PreconditionRequired)
        .ExecuteAsync(context);

        return;
    }
    await next(context);
});

//map endpoints
app.MapCafeEndpoints();
app.MapEmployeeEndpoints();

app.Run();
