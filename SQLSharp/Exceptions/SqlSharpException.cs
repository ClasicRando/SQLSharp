namespace SQLSharp.Exceptions;

/// <summary>
/// Custom exception type for any exception thrown within the SQLSharp library. Delegates the
/// parameters to the base <see cref="Exception"/> class.
/// </summary>
/// <param name="message">exception message</param>
/// <param name="cause">underlining cause of the exception</param>
public class SqlSharpException(string message, Exception? cause = null) : Exception(message, cause)
{
    /// <summary>
    /// Create a templated <see cref="SqlSharpException"/> for when a field is expected to be
    /// non-null but the value is null.
    /// </summary>
    /// <param name="column">column index that was null</param>
    /// <returns>a <see cref="SqlSharpException"/> with templated messages</returns>
    internal static SqlSharpException NullField(int column)
    {
        return new SqlSharpException($"Null value in field #{column}");
    }
}
