using CafeEmployeeApi.Database;
using CafeEmployeeApi.Extensions;
using CafeEmployeeApi.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Contracts.Queries;

public record GetEmployeesRequest : GuidRequest, IRequest<Result<IEnumerable<Employee>>>
{
    public GetEmployeesRequest(string? Cafe) : base(Cafe){
        this.Cafe = Cafe;
    }
    public string? Cafe { get; private set; }
}

public class GetEmployeesRequestHandler : IRequestHandler<GetEmployeesRequest, Result<IEnumerable<Employee>>>
{
    private readonly AppDbContext _dbContext;

    public GetEmployeesRequestHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Result<IEnumerable<Employee>>> Handle(GetEmployeesRequest request, CancellationToken cancellationToken)
    {
        request.Cafe!.TryToGuid(out Guid cafeId);

        var employees = await _dbContext.Employees
            .Where(e => cafeId == Guid.Empty || e.CafeId == cafeId)
            .Include(e => e.AssignedCafe)
            .ToListAsync();

        return Result<IEnumerable<Employee>>.Success(employees);
    }
}

public record GetEmployeesResponse(
    string Id, 
    string Name, 
    string EmailAddress, 
    string PhoneNumber, 
    int DaysWorked, 
    string Cafe);