using CafeEmployeeApi.Contracts;
using CafeEmployeeApi.Contracts.Commands;
using CafeEmployeeApi.Database;
using CafeEmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Services;

public interface IAddService<TEntity, TResponse>
{
    public Task<Result<TResponse>> AddAsync(Func<TEntity> createFunc, Func<TEntity, TResponse> createResponse);
}

public class AddService<TEntity, TResponse> : IAddService<TEntity, TResponse> where TEntity : class
{
    private readonly AppDbContext _dbContext;
    private readonly string _updateExceptionMessage;

    public AddService(
        AppDbContext dbContext,
        string updateExceptionMessage)
    {
        _dbContext = dbContext;
        _updateExceptionMessage = updateExceptionMessage;
    }

    public async Task<Result<TResponse>> AddAsync(
        Func<TEntity> createFunc, 
        Func<TEntity, TResponse> createResponse)
    {
        var newEntity = createFunc();
        try
        {
            _dbContext.Set<TEntity>().Add(newEntity);
            await _dbContext.SaveChangesAsync();

            return Result<TResponse>.Success(createResponse(newEntity));
        }
        catch (DbUpdateException ex)
        {
            //TODO: add logging
            return Result<TResponse>.Failure(_updateExceptionMessage);
        }
    }
}

public class AddCafeService : AddService<Cafe, CreateCafeResponse>
{
    public AddCafeService(AppDbContext dbContext) 
    : base(dbContext, "The cafe already exists in the location") { }
}

public class AddEmployeeService : AddService<Employee, CreateEmployeeResponse>
{
    public AddEmployeeService(AppDbContext dbContext) 
    : base(dbContext, "The employee already exists") { }
}
