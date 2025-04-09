using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Models;

[PrimaryKey(nameof(EmployeeId), nameof(CafeId))]
public class EmploymentHistory
{
    public string EmployeeId { get; set; } = string.Empty;
    public Employee Employee { get; set; } = null!;
    public Guid CafeId { get; set; }
    public Cafe Cafe { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}