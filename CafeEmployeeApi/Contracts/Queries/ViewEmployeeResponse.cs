namespace CafeEmployeeApi.Contracts.Queries;
public record ViewEmployeeResponse(
    string Id, 
    string Name, 
    string EmailAddress, 
    string PhoneNumber, 
    int Gender,
    int DaysWorked, 
    Guid? AssignedCafeId);