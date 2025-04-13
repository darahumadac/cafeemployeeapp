namespace CafeEmployeeApi.Extensions;

public static partial class EndpointExtensions
{
    public static void MapCafeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var cafes = endpoints.MapGroup("/cafes");

        cafes.MapGet("/", GetCafesAsync)
            .WithName("GetCafes")
            .CacheOutput(builder => builder.Expire(TimeSpan.FromSeconds(3)));
            

        cafes.MapGet("/{id}", GetCafeAsync)
            .WithName("GetCafe")
            .CacheOutput(builder => builder.Expire(TimeSpan.FromSeconds(5)));


        var cafeEndpoints  = endpoints.MapGroup("/cafe");
        
        cafeEndpoints.MapPost("/", AddCafeAsync)
            .WithName("AddCafe");
        
        cafeEndpoints.MapPut("/{id}", UpdateCafeAsync)
            .WithName("UpdateCafe");

        cafeEndpoints.MapDelete("/{id}", DeleteCafeAsync)
            .WithName("DeleteCafe");
    }

    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var employees = endpoints.MapGroup("/employees");

        employees.MapGet("/", GetEmployeesAsync)
            .WithName("GetEmployees")
            .CacheOutput(builder => builder.Expire(TimeSpan.FromSeconds(3)));

        employees.MapGet("/{id}", GetEmployeeAsync)
            .WithName("GetEmployee")
            .CacheOutput(builder => builder.Expire(TimeSpan.FromSeconds(5)));

        var employee = endpoints.MapGroup("/employee");

        employee.MapPost("/", AddEmployeeAsync)
            .WithName("AddEmployee");

        employee.MapPut("/{id}", UpdateEmployeeAsync)
            .WithName("UpdateEmployee");

        employee.MapDelete("/{id}", DeleteEmployeeAsync)
            .WithName("DeleteEmployee");
        


    }
    
}