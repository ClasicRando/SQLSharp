using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlDateTimeOffset : IDbDecode<DateTimeOffset?>
{
    public static DateTimeOffset? Decode(IDataRow row, int column)
    {
        return row.GetField<DateTimeOffset>(column);
    }
}