using System.Data;
using SQLSharp.Command;
using SQLSharp.Exceptions;
using SQLSharp.Result;
using SQLSharp.Types;

namespace SQLSharp.Extensions;

/// <summary>
/// Extension methods for the <see cref="IDbConnection"/> interface. Provide wrapper methods for
/// common operations and integrate with the <see cref="IFromRow{T}"/> and
/// <see cref="IDbDecode{TResult}"/> interfaces.
/// </summary>
public static class ConnectionExtensions
{
    /// <summary>
    /// Execute the supplied query against this connection and extracts the first row's first column
    /// value and return it. This differs from <see cref="ConnectionExtensions.QueryScalar{T}"/>
    /// because the return type must implement <see cref="IDbDecode{TResult}"/> for itself.
    /// </summary>
    /// <param name="connection">db connection to execute query against</param>
    /// <param name="query">SQL query to execute</param>
    /// <param name="parameters">optional parameters to supply to the query for execution</param>
    /// <param name="transaction">
    /// optional transaction to execute the query within, default is autocommit mode
    /// </param>
    /// <param name="queryTimeout">
    /// optional query timeout, overrides the default connection property if specified
    /// </param>
    /// <param name="commandType">
    /// optional query command type, default is <see cref="CommandType.Text"/>
    /// </param>
    /// <typeparam name="T">scalar decode result type</typeparam>
    /// <returns>
    /// the value of the first row's first column value, null if <typeparamref name="T"/> is a
    /// reference type and the DB value is null, default value if <typeparamref name="T"/> is a
    /// value type and DB value is null
    /// </returns>
    public static T? QueryScalarDecode<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IDbDecode<T>
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryScalarImpl<T, T>(command);
    }

    /// <summary>
    /// Execute the supplied query against this connection and extracts the first row's first column
    /// value and return it. This differs from
    /// <see cref="ConnectionExtensions.QueryScalarDecode{T}"/> because the return type does not
    /// need <see cref="IDbDecode{TResult}"/> for itself. Use this when it's a built-in or external
    /// type that can be deserialized automatically from a returned row.
    /// </summary>
    /// <param name="connection">db connection to execute query against</param>
    /// <param name="query">SQL query to execute</param>
    /// <param name="parameters">optional parameters to supply to the query for execution</param>
    /// <param name="transaction">
    /// optional transaction to execute the query within, default is autocommit mode
    /// </param>
    /// <param name="queryTimeout">
    /// optional query timeout, overrides the default connection property if specified
    /// </param>
    /// <param name="commandType">
    /// optional query command type, default is <see cref="CommandType.Text"/>
    /// </param>
    /// <typeparam name="T">scalar result type</typeparam>
    /// <returns>
    /// the value of the first row's first column value, null if <typeparamref name="T"/> is a
    /// reference type and the DB value is null, default value if <typeparamref name="T"/> is a
    /// value type and DB value is null
    /// </returns>
    public static T? QueryScalar<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null)
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryScalarImpl<AnyDbDecode<T>, T>(command);
    }

    /// <summary>
    /// Internal implementation of querying a scalar value and deserializing to
    /// <typeparamref name="TResult"/>
    /// </summary>
    /// <param name="command">command properties</param>
    /// <typeparam name="TDecoder">
    /// decoder type that deserializes to <typeparamref name="TResult"/>
    /// </typeparam>
    /// <typeparam name="TResult">result type of deserialization</typeparam>
    /// <returns>
    /// scalar value as the first row's first column, <typeparamref name="TResult"/>'s default value
    /// if the value is a DB null
    /// </returns>
    private static TResult? QueryScalarImpl<TDecoder, TResult>(
        SqlSharpCommand<IDbConnection, IDbTransaction> command) where TDecoder : IDbDecode<TResult>
    {
        var row = QueryRow<ScalarResultRow<TDecoder, TResult>>(command, isSingleRowOnly: false);
        if (row is null)
        {
            return default;
        }

        return row.Inner ?? default;
    }

    /// <summary>
    /// Execute the supplied query against this connection and extracts the first row as a new
    /// instance of <typeparamref name="T"/>. Query must return exactly 1 row.
    /// </summary>
    /// <param name="connection">db connection to execute query against</param>
    /// <param name="query">SQL query to execute</param>
    /// <param name="parameters">optional parameters to supply to the query for execution</param>
    /// <param name="transaction">
    /// optional transaction to execute the query within, default is autocommit mode
    /// </param>
    /// <param name="queryTimeout">
    /// optional query timeout, overrides the default connection property if specified
    /// </param>
    /// <param name="commandType">
    /// optional query command type, default is <see cref="CommandType.Text"/>
    /// </param>
    /// <typeparam name="T">type that can deserialize a row into itself</typeparam>
    /// <returns>the result of deserializing a result row into <typeparamref name="T"/></returns>
    /// <exception cref="SqlSharpException">
    /// if the query returns a result without exactly 1 row
    /// </exception>
    public static T QuerySingle<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        return connection.QuerySingleOrNull<T>(
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType) ?? throw new SqlSharpException("Expected exactly 1 row but found none");
    }

    /// <summary>
    /// Execute the supplied query against this connection and extracts the first row as a new
    /// instance of <typeparamref name="T"/>. Query must return exactly 0 or 1 row.
    /// </summary>
    /// <param name="connection">db connection to execute query against</param>
    /// <param name="query">SQL query to execute</param>
    /// <param name="parameters">optional parameters to supply to the query for execution</param>
    /// <param name="transaction">
    /// optional transaction to execute the query within, default is autocommit mode
    /// </param>
    /// <param name="queryTimeout">
    /// optional query timeout, overrides the default connection property if specified
    /// </param>
    /// <param name="commandType">
    /// optional query command type, default is <see cref="CommandType.Text"/>
    /// </param>
    /// <typeparam name="T">type that can deserialize a row into itself</typeparam>
    /// <returns>
    /// the result of deserializing a result row into <typeparamref name="T"/> or null/default if no
    /// rows were returned
    /// </returns>
    /// <exception cref="SqlSharpException">if the query returns more than 1 row</exception>
    public static T? QuerySingleOrNull<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryRow<T>(command, isSingleRowOnly: true);
    }

    /// <summary>
    /// Execute the supplied query against this connection and extracts the first row as a new
    /// instance of <typeparamref name="T"/>. Query can return multiple rows but only the first row
    /// is used.
    /// </summary>
    /// <param name="connection">db connection to execute query against</param>
    /// <param name="query">SQL query to execute</param>
    /// <param name="parameters">optional parameters to supply to the query for execution</param>
    /// <param name="transaction">
    /// optional transaction to execute the query within, default is autocommit mode
    /// </param>
    /// <param name="queryTimeout">
    /// optional query timeout, overrides the default connection property if specified
    /// </param>
    /// <param name="commandType">
    /// optional query command type, default is <see cref="CommandType.Text"/>
    /// </param>
    /// <typeparam name="T">type that can deserialize a row into itself</typeparam>
    /// <returns>the result of deserializing a result row into <typeparamref name="T"/></returns>
    /// <exception cref="SqlSharpException">if the query returns a result with 0 rows</exception>
    public static T QueryFirst<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        return connection.QueryFirstOrNull<T>(
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType) ?? throw new SqlSharpException("Expected at least 1 row but found zero");
    }

    /// <summary>
    /// Execute the supplied query against this connection and extracts the first row as a new
    /// instance of <typeparamref name="T"/>. Query can return any number of rows including 0.
    /// </summary>
    /// <param name="connection">db connection to execute query against</param>
    /// <param name="query">SQL query to execute</param>
    /// <param name="parameters">optional parameters to supply to the query for execution</param>
    /// <param name="transaction">
    /// optional transaction to execute the query within, default is autocommit mode
    /// </param>
    /// <param name="queryTimeout">
    /// optional query timeout, overrides the default connection property if specified
    /// </param>
    /// <param name="commandType">
    /// optional query command type, default is <see cref="CommandType.Text"/>
    /// </param>
    /// <typeparam name="T">type that can deserialize a row into itself</typeparam>
    /// <returns>
    /// the result of deserializing a result row into <typeparamref name="T"/> or null/default if no
    /// rows were returned
    /// </returns>
    public static T? QueryFirstOrNull<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryRow<T>(command, isSingleRowOnly: false);
    }

    /// <summary>
    /// Internal implementation for executing a query and deserializing the first row to
    /// <typeparamref name="T"/>. Includes optimizations and simplifications when compared to
    /// <see cref="QueryImpl{T}"/>.
    /// </summary>
    /// <param name="sqlSharpCommand">command properties</param>
    /// <param name="isSingleRowOnly">true if multiple rows returned should throw</param>
    /// <typeparam name="T">deserialization result type</typeparam>
    /// <returns>enumerable of the database rows mapped to <typeparamref name="T"/></returns>
    /// <exception cref="SqlSharpException">
    /// if <paramref name="isSingleRowOnly"/> is true and multiple rows are returned
    /// </exception>
    private static T? QueryRow<T>(
        SqlSharpCommand<IDbConnection, IDbTransaction> sqlSharpCommand,
        bool isSingleRowOnly) where T : IFromRow<T>
    {
        var wasClosed = sqlSharpCommand.Connection.State == ConnectionState.Closed;

        try
        {
            if (wasClosed)
            {
                sqlSharpCommand.Connection.Open();
            }

            using IDbCommand command = sqlSharpCommand.Connection.CreateCommand();
            command.PrepareCommand(sqlSharpCommand);

            using IDataReader reader = command.ExecuteReader();
            if (!reader.Read()) return default;

            var row = new SqlSharpDataRow(reader);
            T result = T.FromRow(row);

            if (isSingleRowOnly && reader.Read())
            {
                throw new SqlSharpException("Expected exactly 1 row but found more than 1");
            }

            return result;
        }
        finally
        {
            if (wasClosed)
            {
                sqlSharpCommand.Connection.Close();
            }
        }
    }

    /// <summary>
    /// Execute the supplied query against this connection and set up an enumeration of the result
    /// set to deserialize each row into <typeparamref name="T"/>. The result is lazy so each call
    /// to <see cref="IEnumerator{T}.MoveNext"/> will read the next row from the result. Also note
    /// that the command will be open until the result is exhausted so the connection might be
    /// locked until that happens.
    /// </summary>
    /// <param name="connection">db connection to execute query against</param>
    /// <param name="query">SQL query to execute</param>
    /// <param name="parameters">optional parameters to supply to the query for execution</param>
    /// <param name="transaction">
    /// optional transaction to execute the query within, default is autocommit mode
    /// </param>
    /// <param name="queryTimeout">
    /// optional query timeout, overrides the default connection property if specified
    /// </param>
    /// <param name="commandType">
    /// optional query command type, default is <see cref="CommandType.Text"/>
    /// </param>
    /// <typeparam name="T">type that can deserialize a row into itself</typeparam>
    /// <returns>
    /// the result of deserializing each result row into <typeparamref name="T"/> as a lazy result
    /// </returns>
    public static IEnumerable<T> Query<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryImpl<T>(command);
    }

    /// <summary>
    /// Execute the supplied query against this connection and collect a <see cref="List{T}"/> by
    /// deserializing each row into <typeparamref name="T"/>
    /// </summary>
    /// <param name="connection">db connection to execute query against</param>
    /// <param name="query">SQL query to execute</param>
    /// <param name="parameters">optional parameters to supply to the query for execution</param>
    /// <param name="transaction">
    /// optional transaction to execute the query within, default is autocommit mode
    /// </param>
    /// <param name="queryTimeout">
    /// optional query timeout, overrides the default connection property if specified
    /// </param>
    /// <param name="commandType">
    /// optional query command type, default is <see cref="CommandType.Text"/>
    /// </param>
    /// <typeparam name="T">type that can deserialize a row into itself</typeparam>
    /// <returns>
    /// the result of deserializing each result row into <typeparamref name="T"/>
    /// </returns>
    public static List<T> QueryAll<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        return Query<T>(
                connection,
                query,
                parameters,
                transaction,
                queryTimeout,
                commandType)
            .ToList();
    }

    /// <summary>
    /// Internal implementation of query execution that returns a generator of rows mapped using
    /// <see cref="T.FromRow"/>. Make sure to either exhausted the enumerable to close underlining
    /// resources or clearly state in documentation that you must exhaust the enumerable.
    /// </summary>
    /// <param name="sqlSharpCommand">command properties</param>
    /// <typeparam name="T">deserialization result type</typeparam>
    /// <returns>enumerable of the database rows mapped to <typeparamref name="T"/></returns>
    private static IEnumerable<T> QueryImpl<T>(
        SqlSharpCommand<IDbConnection, IDbTransaction> sqlSharpCommand) where T : IFromRow<T>
    {
        var wasClosed = sqlSharpCommand.Connection.State == ConnectionState.Closed;
        try
        {
            if (wasClosed)
            {
                sqlSharpCommand.Connection.Open();
            }

            using IDbCommand command = sqlSharpCommand.Connection.CreateCommand();
            command.PrepareCommand(sqlSharpCommand);

            using IDataReader reader = command.ExecuteReader();
            if (!reader.Read()) yield break;

            do
            {
                var row = new SqlSharpDataRow(reader);
                yield return T.FromRow(row);
            } while (reader.Read());
        }
        finally
        {
            if (wasClosed)
            {
                sqlSharpCommand.Connection.Close();
            }
        }
    }

    /// <summary>
    /// Execute the supplied query against this connection and fetch the number of rows impacted
    /// </summary>
    /// <param name="connection">db connection to execute query against</param>
    /// <param name="query">SQL query to execute</param>
    /// <param name="parameters">optional parameters to supply to the query for execution</param>
    /// <param name="transaction">
    /// optional transaction to execute the query within, default is autocommit mode
    /// </param>
    /// <param name="queryTimeout">
    /// optional query timeout, overrides the default connection property if specified
    /// </param>
    /// <param name="commandType">
    /// optional query command type, default is <see cref="CommandType.Text"/>
    /// </param>
    /// <returns>the number of rows impacted by the statement</returns>
    public static int Execute(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null)
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return Execute(command);
    }

    /// <summary>
    /// Internal implementation for executing a non-query or DML statement. Prepares the command and
    /// executes the non-query, returning the number of impacted rows.
    /// </summary>
    /// <param name="sqlSharpCommand">command properties</param>
    /// <returns>the number of rows impacted by the statement</returns>
    private static int Execute(SqlSharpCommand<IDbConnection, IDbTransaction> sqlSharpCommand)
    {
        var wasClosed = sqlSharpCommand.Connection.State == ConnectionState.Closed;

        try
        {
            if (wasClosed)
            {
                sqlSharpCommand.Connection.Open();
            }

            using IDbCommand command = sqlSharpCommand.Connection.CreateCommand();
            command.PrepareCommand(sqlSharpCommand);

            return command.ExecuteNonQuery();
        }
        finally
        {
            if (wasClosed)
            {
                sqlSharpCommand.Connection.Close();
            }
        }
    }
}
