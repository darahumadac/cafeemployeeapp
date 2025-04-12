using CafeEmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions) { }
    public DbSet<Cafe> Cafes { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //set cafe id
        modelBuilder.Entity<Cafe>()
            .Property(c => c.Id)
            .HasDefaultValueSql("NEWID()");

        //set the id generator
        modelBuilder.Entity<Employee>()
            .Property(e => e.Id)
            .HasValueGenerator<EmployeeIdGenerator>();

        //set created and updated dates
        modelBuilder.Entity<Cafe>()
            .Property(c => c.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Cafe>()
            .Property(c => c.UpdatedDate)
            .HasDefaultValueSql("GETUTCDATE()");
            
        modelBuilder.Entity<Employee>()
            .Property(e => e.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Employee>()
            .Property(e => e.UpdatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        //etag generator
        modelBuilder.Entity<Cafe>()
            .Property(c => c.ETag)
            .IsRowVersion();

        modelBuilder.Entity<Employee>()
            .Property(e => e.ETag)
            .IsRowVersion();
        

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((dbContext, _) =>
        {
            var random = new Random();
            //seed cafes
            var cafes = dbContext.Set<Cafe>();
            if (cafes.Count() == 0)
            {
                var addCafes = Enumerable.Range(1, 5).Select(n =>
                    new Cafe
                    {
                        Name = $"Cafe {n}",
                        Description = $"Best cafe in town - {n}",
                        Location = random.GetItems(["Singapore", "US", "Philippines"], 1)[0]
                    });
                cafes.AddRange(addCafes);
                dbContext.SaveChanges();
            }            

            //seed employees
            var employees = dbContext.Set<Employee>();
            if (employees.Count() == 0)
            {
                //assigned
                var assignedEmployees = Enumerable.Range(1, 5).Select(n => new Employee
                {
                    Name = $"Employee {n}",
                    Email = $"employee{n}@cafe.com",
                    PhoneNumber = $"8234567{n}",
                    Gender = Convert.ToBoolean(random.Next(0, 2)),
                    CafeId = random.GetItems(cafes.ToArray(), 1)[0].Id,
                    StartDate = DateTime.UtcNow.AddDays(random.Next(-10, -6))
                });
                employees.AddRange(assignedEmployees);

                //unassigned
                employees.AddRange(
                    Enumerable.Range(1, 5).Select(n => new Employee
                    {
                        Name = $"Employee Unassigned ",
                        Email = $"employee{n}@cafe.com",
                        PhoneNumber = $"9234567{n}",
                        Gender = Convert.ToBoolean(random.Next(0, 2)),
                    }));

                dbContext.SaveChanges();
            }
        });
    }

}