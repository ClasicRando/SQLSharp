using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlLong : IDbDecode<long?>
{
    public static long? Decode(IDataRow row, int column)
    {
        return row.GetField<long>(column);
    }
}
