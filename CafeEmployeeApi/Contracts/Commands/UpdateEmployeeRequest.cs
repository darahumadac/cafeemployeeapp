namespace CafeEmployeeApi.Contracts.Commands;
public record UpdateEmployeeRequest(
    string Id, 
    string Name, 
    string EmailAddress, 
    string PhoneNumber, 
    bool Gender, 
    Guid? AssignedCafeId);