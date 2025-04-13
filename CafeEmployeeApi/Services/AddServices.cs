using CafeEmployeeApi.Contracts;
using CafeEmployeeApi.Database;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Services;

public interface IAddService<TEntity, TResponse>
{
    public Task<Result<TResponse>> AddAsync();
    public Func<TEntity> CreateEntity { get; set; }
    public Func<TEntity, TResponse> CreateResponse { get; set; }
}

public class AddService<TEntity, TResponse> : IAddService<TEntity, TResponse> 
    where TEntity : class
{
    private readonly AppDbContext _dbContext;

    public AddService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Func<TEntity> CreateEntity { get; set; } = null!;
    public Func<TEntity, TResponse> CreateResponse { get; set; } = null!

    public async Task<Result<TResponse>> AddAsync()
    {
        TEntity newEntity = CreateEntity();
        try
        {
            _dbContext.Set<TEntity>().Add(newEntity);
            await _dbContext.SaveChangesAsync();

            return Result<TResponse>.Success(CreateResponse(newEntity));
        }
        catch (DbUpdateException ex)
        {
            return Result<TResponse>.Failure(ex.Message);
        }
    }
}