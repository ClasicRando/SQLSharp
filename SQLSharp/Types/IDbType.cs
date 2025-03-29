namespace SQLSharp.Types;

public interface IDbType<out TResult> : IDbEncode, IDbDecode<TResult>;
