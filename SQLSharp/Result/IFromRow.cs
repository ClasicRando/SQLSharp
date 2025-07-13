namespace SQLSharp.Result;

/// <summary>
/// Interface to allow deserializing a <see cref="IDataRow"/> into a specified type
/// <typeparamref name="T"/> (which is generally the type itself).
/// </summary>
/// <typeparam name="T">result type after deserialization</typeparam>
public interface IFromRow<out T>
{
    /// <summary>
    /// Convert the contents of the supplied row to a new instance of <typeparamref name="T"/>
    /// </summary>
    /// <param name="row">database row to deserialize</param>
    /// <returns>
    /// a new instance of the <typeparamref name="T"/> with the contents of the row
    /// </returns>
    public static abstract T FromRow(IDataRow row);
}
