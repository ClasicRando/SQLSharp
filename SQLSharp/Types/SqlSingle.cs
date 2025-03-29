using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlSingle : IDbDecode<float?>
{
    public static float? Decode(IDataRow row, int column)
    {
        return row.GetField<float>(column);
    }
}
