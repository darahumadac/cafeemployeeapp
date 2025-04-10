namespace CafeEmployeeApi.Contracts.Queries;

public record GetCafesResponse(
    string Name, 
    string Description, 
    int Employees, 
    string Location, 
    Guid Id, 
    byte[]? Logo = null);

