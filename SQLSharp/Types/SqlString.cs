using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlString : IDbDecode<string?>
{
    public static string? Decode(IDataRow row, int column)
    {
        return row.GetFieldAsClass<string>(column);
    }
}