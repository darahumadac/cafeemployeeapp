namespace CafeEmployeeApi.Extensions;

public static class StringExtensions
{
    public static bool TryToGuid(this string id, out Guid guid)
    {
        guid = Guid.Empty;
        if(!string.IsNullOrEmpty(id))
        {
            return Guid.TryParse(id, out guid);
        }
        return false;
    }
}