using System.Data;

namespace SQLSharp.Types;

/// <summary>
/// Interface that defines a type that can be encoded itself into a database command parameter.
/// </summary>
public interface IDbEncode
{
    /// <summary>
    /// Serialize this instance into the parameter provided by the method. Set all properties of the
    /// parameter that are needed.
    /// </summary>
    /// <param name="parameter">reference to a parameter to populate</param>
    public void Encode(ref IDbDataParameter parameter);
}
