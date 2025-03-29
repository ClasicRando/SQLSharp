using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlDecimal : IDbDecode<decimal?>
{
    public static decimal? Decode(IDataRow row, int column)
    {
        return row.GetField<decimal>(column);
    }
}