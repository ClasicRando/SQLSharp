using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlByte : IDbDecode<byte?>
{
    public static byte? Decode(IDataRow row, int column)
    {
        return row.GetField<byte>(column);
    }
}
