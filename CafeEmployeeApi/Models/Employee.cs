using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Models;

[Index(nameof(Name), nameof(Email), nameof(PhoneNumber), IsUnique = true)]
public class Employee : Auditable
{
    //UIXXXXXXX -> X is alphanumeric
    public string Id { get; set; } = null!;
 
    [Required]
    [MaxLength(50)] //in db set to 50, just in case;
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MinLength(8)]
    [MaxLength(8)]
    [RegularExpression(@"^[8|9][0-9]{7}$")]
    public string PhoneNumber { get; set; } = string.Empty; //starts with 8 or 9 and have 8 digits

    [Required]
    public bool Gender { get; set; } //0 - Male, 1 - Female

    public Guid? CafeId { get; set; }
    public Cafe? AssignedCafe { get; set; }
    public DateTime? StartDate { get; set; } = null!;

    [NotMapped]
    public int DaysWorked => StartDate == null ? 0 : DateTime.UtcNow.Subtract((DateTime)StartDate!).Days;
}