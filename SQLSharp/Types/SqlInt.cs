using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlInt : IDbDecode<int?>
{
    public static int? Decode(IDataRow row, int column)
    {
        return row.GetField<int>(column);
    }
}
