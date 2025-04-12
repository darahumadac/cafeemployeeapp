namespace CafeEmployeeApi.Contracts.Commands;

public record CreateCafeResponse(
    Guid Id,
    string Name, 
    string Description, 
    string Location, 
    string ETag,
    byte[]? Logo = null);