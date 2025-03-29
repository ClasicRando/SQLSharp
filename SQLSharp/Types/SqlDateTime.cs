using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlDateTime : IDbDecode<DateTime?>
{
    public static DateTime? Decode(IDataRow row, int column)
    {
        return row.GetField<DateTime>(column);
    }
}