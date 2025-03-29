using SQLSharp.Result;

namespace SQLSharp.Types;

public readonly struct SqlGuid : IDbDecode<Guid?>
{
    public static Guid? Decode(IDataRow row, int column)
    {
        return row.GetField<Guid>(column);
    }
}