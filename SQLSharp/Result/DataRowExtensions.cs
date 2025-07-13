using SQLSharp.Exceptions;

namespace SQLSharp.Result;

/// <summary>
/// Extension methods on <see cref="IDataRow"/> to extract column values to specific types
/// </summary>
public static class DataRowExtensions
{
    /// <summary>
    /// Get the value behind the column found by index, allowing returns of null when the
    /// underlining value is null or DBNull.Value. Note that when <typeparamref name="T"/> is a
    /// non-null value type, this method never returns null but rather the types default value.
    /// </summary>
    /// <param name="dataRow">row to extract from</param>
    /// <param name="index">0-based column index to fetch from</param>
    /// <typeparam name="T">field type to extract as</typeparam>
    /// <returns>
    /// The value behind the column index or the types default value if the value is null or
    /// DBNull.Value
    /// </returns>
    /// <exception cref="SqlSharpException">
    /// if the extracted value is not <typeparamref name="T"/> and it cannot be implicitly converted
    /// to <typeparamref name="T"/> using <see cref="Convert.ChangeType(object?, Type)"/>
    /// </exception>
    public static T? GetField<T>(this IDataRow dataRow, int index)
    {
        var value = dataRow[index];
        switch (value)
        {
            case null:
                return default;
            case T v:
                return v;
            default:
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception e)
                {
                    throw new SqlSharpException(
                        $"Cannot convert field value of {value.GetType()} into {typeof(T)}",
                        e);
                }
        }
    }

    /// <summary>
    /// Get the value behind the column found by index, disallowing a return of null when the
    /// underlining value is null or DBNull.Value.
    /// </summary>
    /// <param name="dataRow">row to extract from</param>
    /// <param name="index">0-based column index to fetch from</param>
    /// <typeparam name="T">field type to extract as</typeparam>
    /// <returns>The value behind the column index, will never be null</returns>
    /// <exception cref="SqlSharpException">
    /// if the extracted value is not <typeparamref name="T"/> and it cannot be implicitly converted
    /// to <typeparamref name="T"/> using <see cref="Convert.ChangeType(object?, Type)"/>, or if the
    /// underlining value is null or DBNull.Value
    /// </exception>
    public static T GetFieldNotNull<T>(this IDataRow dataRow, int index)
    {
        var value = dataRow[index];
        switch (value)
        {
            case null:
                throw SqlSharpException.NullField(index);
            case T v:
                return v;
            default:
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception e)
                {
                    throw new SqlSharpException(
                        $"Cannot convert field value of {value.GetType()} into {typeof(T)}",
                        e);
                }
        }
    }

    /// <summary>
    /// Get the value behind the column found by index, allowing returns of null when the
    /// underlining value is null or DBNull.Value. Equivalent to calling:
    /// <code>
    /// dataRow.GetField&lt;T&gt;(dataRow.IndexOf(fieldName))
    /// </code>
    /// </summary>
    /// <param name="dataRow">row to extract from</param>
    /// <param name="fieldName">name of the field to extract</param>
    /// <typeparam name="T">field type to extract as</typeparam>
    /// <returns>
    /// The value behind the column index or null if the value is null or DBNull.Value
    /// </returns>
    /// <exception cref="SqlSharpException">
    /// if the extracted value is not <typeparamref name="T"/> and it cannot be implicitly converted
    /// to <typeparamref name="T"/> using <see cref="Convert.ChangeType(object?, Type)"/>
    /// </exception>
    /// <seealso cref="GetField{T}(SQLSharp.Result.IDataRow,int)"/>
    public static T? GetField<T>(this IDataRow dataRow, string fieldName) => dataRow.GetField<T>(dataRow.IndexOf(fieldName));

    /// <summary>
    /// Get the value behind the column found by index, disallowing a return of null when the
    /// underlining value is null or DBNull.Value. Equivalent to calling:
    /// <code>
    /// dataRow.GetFieldNotNull&lt;T&gt;(dataRow.IndexOf(fieldName))
    /// </code>
    /// </summary>
    /// <param name="dataRow">row to extract from</param>
    /// <param name="fieldName">name of the field to extract</param>
    /// <typeparam name="T">field type to extract as</typeparam>
    /// <returns>The value behind the column index, will never be null</returns>
    /// <exception cref="SqlSharpException">
    /// if the extracted value is not <typeparamref name="T"/> and it cannot be implicitly converted
    /// to <typeparamref name="T"/> using <see cref="Convert.ChangeType(object?, Type)"/>, or if the
    /// underlining value is null or DBNull.Value
    /// </exception>
    /// <seealso cref="GetFieldNotNull{T}(SQLSharp.Result.IDataRow,int)"/>
    public static T GetFieldNotNull<T>(this IDataRow dataRow, string fieldName) => dataRow.GetFieldNotNull<T>(dataRow.IndexOf(fieldName));
}
