namespace SQLSharp.Exceptions;

public class SqlSharpException(string message, Exception? cause = null) : Exception(message, cause)
{
    public static SqlSharpException NullField(int column)
    {
        return new SqlSharpException($"Null value in field #{column}");
    }
}
