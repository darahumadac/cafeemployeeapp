using System.Numerics;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace CafeEmployeeApi.Database;

public class EmployeeIdGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry)
    {
        var guid = BitConverter.ToInt128(Guid.NewGuid().ToByteArray());
        return $"UI{base62(guid)}";
    }
    private readonly string _validChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private string base62(BigInteger id)
    {
        string result = string.Empty;
        var quotient = id;
        while (quotient != 0)
        {
            quotient = BigInteger.DivRem(quotient, 62, out var remainder);
            result = $"{_validChars[(int)remainder]}{result}";
            quotient /= 62;
        }
        return result;
    }
}