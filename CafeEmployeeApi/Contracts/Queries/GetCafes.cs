using CafeEmployeeApi.Database;
using CafeEmployeeApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Contracts.Queries;

public record GetCafesRequest(string? Location) : IRequest<IEnumerable<Cafe>>;

public class GetCafesRequestHandler : IRequestHandler<GetCafesRequest, IEnumerable<Cafe>>
{
    private readonly AppDbContext _dbContext;

    public GetCafesRequestHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Cafe>> Handle(GetCafesRequest request, CancellationToken cancellationToken)
    {
        return await _dbContext.Cafes
            .Where(c => string.IsNullOrEmpty(request.Location) || c.Location == request.Location)
            .Include(c => c.Employees)
            .OrderByDescending(c => c.Employees.Count)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }
}

public record GetCafesResponse(
    string Name,
    string Description,
    int Employees,
    string Location,
    Guid Id,
    byte[]? Logo = null);

