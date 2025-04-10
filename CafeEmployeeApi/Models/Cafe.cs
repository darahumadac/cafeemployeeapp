using System.ComponentModel.DataAnnotations;
using CafeEmployeeApi.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Models;

[Index(nameof(Name), nameof(Location), IsUnique = true)]
public class Cafe : Auditable
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _location = string.Empty;

    public Guid Id { get; set; }

    [Required]
    [MinLength(6)]
    [MaxLength(10)]
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value.Sanitize();
        }
    }

    [Required]
    [MaxLength(256)]
    public string Description
    {
        get
        {
            return _description;
        }
        set
        {
            _description = value.Sanitize();
        }
    }

    public byte[]? Logo { get; set; }

    [Required]
    [MaxLength(100)]
    public string Location
    {
        get
        {
            return _location;
        }
        set
        {
            _location = value.Sanitize();
        }
    }

    public ICollection<Employee> Employees { get; set; } = null!;

}