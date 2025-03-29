using System.Data;

namespace SQLSharp.Types;

public interface IDbEncode
{
    public void Encode(ref IDbDataParameter parameter);
}
