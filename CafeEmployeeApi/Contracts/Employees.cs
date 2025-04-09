namespace CafeEmployeeApi.Contracts;

public record EmployeesResponse(string Id, string Name, string EmailAddress, int DaysWorked, string Cafe);

public record AddUpEmployeeRequest(string Name, string EmailAddress, string PhoneNumber, bool Gender, Guid? AssignedCafeId);
public record UpdateEmployeeRequest(string Id, string Name, string EmailAddress, string PhoneNumber, bool Gender, Guid? AssignedCafeId);