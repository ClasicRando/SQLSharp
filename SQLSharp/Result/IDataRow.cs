namespace SQLSharp.Result;

/// <summary>
/// Implementation represent a database query result row. Allows for translating a field name to an
/// index and accessing field values by index or by field name.
/// </summary>
public interface IDataRow
{
    /// <summary>
    /// Find the index for this field name
    /// </summary>
    /// <param name="fieldName">name of the field to associate with an index</param>
    /// <returns>0-based index for the supplied field name, -1 if field not present</returns>
    public int IndexOf(string fieldName);

    /// <summary>
    /// Access the field value for this index, will be null when the database value is null
    /// </summary>
    /// <param name="index">0-based colum index</param>
    public object? this[int index] { get; }

    /// <summary>
    /// Access the field name for this name, will be null when the database value is null
    /// </summary>
    /// <param name="fieldName">field name</param>
    public object? this[string fieldName] => this[IndexOf(fieldName)];
}
