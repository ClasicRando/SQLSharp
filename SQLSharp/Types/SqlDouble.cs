using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlDouble : IDbDecode<double?>
{
    public static double? Decode(IDataRow row, int column)
    {
        return row.GetField<double>(column);
    }
}
