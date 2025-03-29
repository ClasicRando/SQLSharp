namespace SQLSharp.Exceptions;

public class SqlSharpException(string message, Exception? cause = null) : Exception(message, cause)
{
    public static SqlSharpException MissingOrNullField(int column)
    {
        return new SqlSharpException($"Null or missing value in field #{column}");
    }
    
    public static SqlSharpException MissingOrNullField(string fieldName)
    {
        return new SqlSharpException($"Null or missing value in field '{fieldName}'");
    }
}
