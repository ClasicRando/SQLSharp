using System.Data;

namespace SQLSharp.Command;

internal class SqlSharpCommand<TConnection, TTransaction> where TConnection : IDbConnection
    where TTransaction : IDbTransaction
{
    internal TConnection Connection { get; }
    internal string Query { get; }
    internal object? Parameters { get; }
    internal TTransaction? Transaction { get; }
    internal int QueryTimeout { get; }
    internal CommandType CommandType { get; }

    internal SqlSharpCommand(
        TConnection connection,
        string query,
        object? parameters,
        TTransaction? transaction,
        int? queryTimeout,
        CommandType? commandType
    )
    {
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        Query = query ?? throw new ArgumentNullException(nameof(query));
        Parameters = parameters;
        Transaction = transaction;
        QueryTimeout = queryTimeout ?? 30;
        CommandType = commandType ?? CommandType.Text;
    }
}