namespace CafeEmployeeApi.Contracts;

public record EmployeesResponse(string Id, string Name, string EmailAddress, string PhoneNumber, int DaysWorked, string Cafe);

public record AddEmployeeRequest(string Name, string EmailAddress, string PhoneNumber, bool Gender, Guid? AssignedCafeId);
public record UpdateEmployeeRequest(string Id, string Name, string EmailAddress, string PhoneNumber, bool Gender, Guid? AssignedCafeId);