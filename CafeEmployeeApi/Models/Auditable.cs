namespace CafeEmployeeApi.Models;

public class Auditable
{
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public byte[] ETag { get; set; } = default!;
}