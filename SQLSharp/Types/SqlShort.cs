using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlShort : IDbDecode<short?>
{
    public static short? Decode(IDataRow row, int column)
    {
        return row.GetField<short>(column);
    }
}
