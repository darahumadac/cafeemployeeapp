namespace CafeEmployeeApi.Contracts.Commands;

public record CreateEmployeeResponse(
    string Id,
    string Name, 
    string Email, 
    string PhoneNumber, 
    int Gender,
    Guid? CafeId);