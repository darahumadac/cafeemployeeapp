using CafeEmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions){}
    public DbSet<Cafe> Cafes { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //set the id generator
        modelBuilder.Entity<Employee>()
            .Property(e => e.Id)
            .HasValueGenerator<EmployeeIdGenerator>();
    }
    
}