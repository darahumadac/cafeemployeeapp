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
        //set cafe id
        modelBuilder.Entity<Cafe>()
            .Property(c => c.Id)
            .HasDefaultValueSql("NEWID()");

        //set the id generator
        modelBuilder.Entity<Employee>()
            .Property(e => e.Id)
            .HasValueGenerator<EmployeeIdGenerator>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((dbContext, _) => {
            //seed cafes
            var cafes = dbContext.Set<Cafe>();
            if(cafes.Count() == 0)
            {
                var addCafes = Enumerable.Range(1,5).Select(n => 
                new Cafe{
                    Name = $"Cafe {n}", 
                    Description = $"Best cafe in town - {n}", 
                    Location = new Random().GetItems(["Singapore", "US", "Philippines"], 1)[0]
                });
                cafes.AddRange(addCafes);
            }
            
            dbContext.SaveChanges();    

            //seed employees
            var random = new Random();
            var employees = dbContext.Set<Employee>();
            if(employees.Count() == 0)
            {
                //assigned
                var addEmployees = Enumerable.Range(1,5).Select(n => new Employee {
                    Name = $"Employee {n}",
                    Email = $"employee{n}@cafe.com",
                    PhoneNumber = "82345678",
                    Gender = Convert.ToBoolean(random.Next(0,2)),
                    CafeId = random.GetItems(cafes.ToArray(), 1)[0].Id,
                });
                employees.AddRange(addEmployees);
                
                //unassigned
                employees.AddRange(Enumerable.Range(6,9).Select(n => new Employee {
                    Name = $"Employee Unassigned ",
                    Email = $"employee{n}@cafe.com",
                    PhoneNumber = "92345678",
                    Gender = Convert.ToBoolean(random.Next(0,2)),
                }));
            
            }
            dbContext.SaveChanges();
            
        });
    }
    
}