using System.ComponentModel.DataAnnotations;

namespace CafeEmployeeApi.Models;

public class Employee
{
    //TODO: generate own Id in format UIXXXXXXX -> X is alphanumeric. use ef core sequences, then get base62
    public string Id { get; set; } = string.Empty;
 
    [Required]
    [MaxLength(50)] //in db set to 50, just in case;
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public int PhoneNumber { get; set; } //starts with 8 or 9 and have 8 digits

    [Required]
    public bool Gender { get; set; } //0 - Male, 1 - Female

    public int CafeId { get; set; }
    public Cafe AssignedCafe { get; set; } = null!;

}