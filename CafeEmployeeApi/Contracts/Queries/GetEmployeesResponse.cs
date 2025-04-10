namespace CafeEmployeeApi.Contracts.Queries;
public record GetEmployeesResponse(
    string Id, 
    string Name, 
    string EmailAddress, 
    string PhoneNumber, 
    int DaysWorked, 
    string Cafe);