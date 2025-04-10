namespace CafeEmployeeApi.Contracts.Queries;

public record ViewCafeResponse(
    string Name, 
    string Description, 
    string Location, 
    Guid Id, 
    byte[]? Logo = null);