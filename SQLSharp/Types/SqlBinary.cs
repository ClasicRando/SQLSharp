using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlBinary : IDbDecode<byte[]?>
{
    public static byte[]? Decode(IDataRow row, int column)
    {
        return row.GetFieldAsClass<byte[]>(column);
    }
}