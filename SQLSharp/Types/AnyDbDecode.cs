using SQLSharp.Result;

namespace SQLSharp.Types;

internal abstract class AnyDbDecode<T> : IDbDecode<T>
{
    public static T Decode(IDataRow row, int column)
    {
        return row.GetField<T>(column);
    }
}