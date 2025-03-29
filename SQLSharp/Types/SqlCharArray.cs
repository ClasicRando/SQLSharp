using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlCharArray : IDbDecode<char[]?>
{
    public static char[]? Decode(IDataRow row, int column)
    {
        return row.GetFieldAsClass<char[]>(column);
    }
}