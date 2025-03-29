using SQLSharp.Result;

namespace SQLSharp.Types;

internal readonly struct SqlBoolean : IDbDecode<bool?>
{
    public static bool? Decode(IDataRow row, int column)
    {
        return row.GetField<bool>(column);
    }
}
