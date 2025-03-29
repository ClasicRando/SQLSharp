using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlULong : IDbDecode<ulong?>
{
    public static ulong? Decode(IDataRow row, int column)
    {
        return row.GetField<ulong>(column);
    }
}
