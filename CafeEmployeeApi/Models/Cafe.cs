using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Models;

[Index(nameof(Name), nameof(Location), IsUnique = true)]
public class Cafe
{
    public Guid Id { get; set; }
    
    [Required]
    [MinLength(6)]
    [MaxLength(10)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string Description { get; set; } = string.Empty;

    public byte[]? Logo { get; set; }

    [Required]
    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    public ICollection<Employee> Employees { get; set; } = null!;

}