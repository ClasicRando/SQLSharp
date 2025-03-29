namespace SQLSharp.Result;

public class SqlSharpDataRow : IDataRow
{
    private readonly List<string> _fieldNames;
    private readonly object?[] _values;

    public SqlSharpDataRow(List<string> fieldNames, object?[] values)
    {
        _fieldNames = fieldNames ?? throw new ArgumentNullException(nameof(fieldNames));
        _values = values ?? throw new ArgumentNullException(nameof(values));
    }
    
    public int IndexOf(string fieldName)
    {
        ArgumentNullException.ThrowIfNull(fieldName);
        return _fieldNames.IndexOf(fieldName);
    }

    public object? this[int index] => _values[index];
}
