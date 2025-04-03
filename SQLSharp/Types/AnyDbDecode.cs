using SQLSharp.Result;

namespace SQLSharp.Types;

public class AnyDbDecode<T> : IDbDecode<T?> where T : struct
{
    public static T? Decode(IDataRow row, int column)
    {
        return row.GetField<T>(column);
    }
}