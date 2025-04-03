using SQLSharp.Result;

namespace SQLSharp.Types;

public class AnyRefDbDecode<T> : IDbDecode<T?> where T : class
{
    public static T? Decode(IDataRow row, int column)
    {
        return row.GetFieldAsClass<T>(column);
    }
}