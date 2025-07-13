using SQLSharp.Result;

namespace SQLSharp.Types;

/// <summary>
/// Interface that defines a type that can deserialize a single column into the resulting type.
/// Generally this interface is defined for itself, but it can be used to deserialize into other
/// types.
/// </summary>
/// <typeparam name="TResult">result type of deserialization</typeparam>
public interface IDbDecode<out TResult>
{
    /// <summary>
    /// Decode the row column into the resulting type. You should only be interacting with the row
    /// column specified, but you can access any other field of the row if needed.
    /// </summary>
    /// <param name="row">row to decode a value from</param>
    /// <param name="column">0-based column index to decode a value from</param>
    /// <returns>
    /// the decoded value from the field or null if the value is a DB null or the default value if
    /// this is a value type
    /// </returns>
    /// <exception cref="SQLSharp.Exceptions.SqlSharpException">
    /// if decoding fails for any reason implementors should throw this exception, however other
    /// exceptions can be thrown
    /// </exception>
    public static abstract TResult? Decode(IDataRow row, int column);
}
