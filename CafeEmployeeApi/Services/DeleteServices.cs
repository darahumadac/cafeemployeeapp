using CafeEmployeeApi.Database;
using CafeEmployeeApi.Models;

namespace CafeEmployeeApi.Services;

public interface IDeleteService<TId>
{
    public Task<bool> DeleteAsync(TId id);
}

public class DeleteService<TEntity, TId> where TEntity : class
{
    private readonly AppDbContext _dbContext;

    public DeleteService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected async Task<bool> DeleteAsync(TId id, Func<AppDbContext, TEntity, Task>? preDeleteAction = null)
    {
        var entities = _dbContext.Set<TEntity>();
        var entity = await entities.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        if (preDeleteAction != null)
        {
            await preDeleteAction(_dbContext, entity);
        }

        entities.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
public class EmployeeDeleteService : DeleteService<Employee, string>, IDeleteService<string>
{
    public EmployeeDeleteService(AppDbContext dbContext) : base(dbContext) { }
    public async Task<bool> DeleteAsync(string id)
    {
        return await base.DeleteAsync(id);
    }
}
public class CafeDeleteService : DeleteService<Cafe, Guid>, IDeleteService<Guid>
{
    public CafeDeleteService(AppDbContext dbContext) : base(dbContext) { }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var handleEmployees = async (AppDbContext dbContext, Cafe cafe) =>
        {
            var now = DateTime.UtcNow;
            await dbContext.Entry(cafe).Collection(c => c.Employees).LoadAsync();
            foreach (var employee in cafe.Employees)
            {
                employee.StartDate = null;
                employee.UpdatedDate = now;
            }
        };

        return await DeleteAsync(id, handleEmployees);
    }
}