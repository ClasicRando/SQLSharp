using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlUInt : IDbDecode<uint?>
{
    public static uint? Decode(IDataRow row, int column)
    {
        return row.GetField<uint>(column);
    }
}
