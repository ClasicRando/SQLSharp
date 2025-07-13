using SQLSharp.Result;

namespace SQLSharp.Types;

/// <summary>
/// Simple decoder for any type where the decoding simply defers to
/// <see cref="DataRowExtensions.GetField{T}(IDataRow, int)"/>. If <typeparamref name="T"/> refers
/// to a non-null value type, the returned value will never be null.
/// </summary>
/// <typeparam name="T">any type to extract from the specified column index</typeparam>
internal abstract class AnyDbDecode<T> : IDbDecode<T>
{
    public static T? Decode(IDataRow row, int column)
    {
        return row.GetField<T>(column);
    }
}
