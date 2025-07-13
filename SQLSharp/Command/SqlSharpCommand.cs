using System.Data;

namespace SQLSharp.Command;

/// <summary>
/// SQL Command object. Generic over all variants of connection and transaction type 
/// </summary>
/// <typeparam name="TConnection">connection type</typeparam>
/// <typeparam name="TTransaction">transaction type</typeparam>
internal class SqlSharpCommand<TConnection, TTransaction> where TConnection : IDbConnection
    where TTransaction : IDbTransaction
{
    internal TConnection Connection { get; }
    internal string Query { get; }
    internal object? Parameters { get; }
    internal TTransaction? Transaction { get; }
    internal int? QueryTimeout { get; }
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
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(query);
        Connection = connection;
        Query = query;
        Parameters = parameters;
        Transaction = transaction;
        QueryTimeout = queryTimeout;
        CommandType = commandType ?? CommandType.Text;
    }
}