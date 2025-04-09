using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Models;

public class Cafe
{
    public Guid Id { get; set; }
    
    [Required]
    [Length(6, 10)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string Description { get; set; } = string.Empty;

    public byte[]? Logo { get; set; }

    public string Location { get; set; } = string.Empty;

    public List<Employee> Employees { get; set; } = null!;

}