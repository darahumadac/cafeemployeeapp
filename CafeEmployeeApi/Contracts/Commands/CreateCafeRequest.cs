namespace CafeEmployeeApi.Contracts.Commands;

public record CreateCafeRequest(
    string Name, 
    string Description, 
    string Location, 
    byte[]? Logo = null);