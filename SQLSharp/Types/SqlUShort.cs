using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlUShort : IDbDecode<ushort?>
{
    public static ushort? Decode(IDataRow row, int column)
    {
        return row.GetField<ushort>(column);
    }
}
