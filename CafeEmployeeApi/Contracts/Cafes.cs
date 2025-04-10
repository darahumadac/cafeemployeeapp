namespace CafeEmployeeApi.Contracts;

public record GetCafesResponse(
    string Name, 
    string Description, 
    int Employees, 
    string Location, 
    Guid Id, 
    byte[]? Logo = null);

public record AddCafeRequest(
    string Name, 
    string Description, 
    string Location, 
    byte[]? Logo = null);

public record ViewCafeResponse(
    string Name, 
    string Description, 
    string Location, 
    Guid Id, 
    byte[]? Logo = null);