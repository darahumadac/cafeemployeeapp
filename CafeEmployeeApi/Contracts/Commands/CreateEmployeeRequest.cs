namespace CafeEmployeeApi.Contracts.Commands;

public record CreateEmployeeRequest(
    string Name, 
    string EmailAddress, 
    string PhoneNumber, 
    bool Gender, 
    Guid? AssignedCafeId);