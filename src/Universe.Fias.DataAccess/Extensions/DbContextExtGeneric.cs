using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Diagnostics;
using System.Linq;

namespace Universe.Fias.DataAccess.Extensions
{
    /// <summary>
    /// Extension <see cref="DbContextExt"/>.
    /// </summary>
    /// <typeparam name="TConn">The type of the connection.</typeparam>
    /// <typeparam name="TCmd">The type of the command.</typeparam>
    /// <typeparam name="TParColl">The type of the par coll.</typeparam>
    /// <typeparam name="TPar">The type of the par.</typeparam>
    /// <typeparam name="TReader">The type of the reader.</typeparam>
    /// <seealso cref="DbContextExt" />
    public class DbContextExt<TConn, TCmd, TParColl, TPar, TReader> : DbContextExt
        where TConn : DbConnection
        where TCmd : DbCommand
        where TParColl : DbParameterCollection
        where TPar : DbParameter
        where TReader : DbDataReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextExt{TConn, TCmd, TParColl, TPar, TReader}"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public DbContextExt(DbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public TConn Connection
        {
            get
            {
                var entityConnection = DbContext.Database.Connection as EntityConnection;
                if (entityConnection != null)
                    return (TConn)entityConnection.StoreConnection;

                return (TConn)DbContext.Database.Connection;
            }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="setParameters">The set parameters.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">func</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public T ExecuteCmd<T>(
            string sql,
            CommandType commandType = CommandType.Text,
            int? commandTimeout = null,
            Action<TParColl> setParameters = null,
            TPar[] parameters = null,
            Func<TConn, TCmd, T> func = null)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return ExecuteConn(
                conn => {
                    using (var cmd = (TCmd)conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = commandType;
                        if (DbContext.Database.CurrentTransaction != null)
                            cmd.Transaction = DbContext.Database.CurrentTransaction.UnderlyingTransaction;

                        setParameters?.Invoke((TParColl)cmd.Parameters);

                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters.Where(_ => _ != null).ToArray());

                        if (commandTimeout != null)
                            cmd.CommandTimeout = commandTimeout.Value;

                        return func(conn, cmd);
                    }
                });
        }

        /// <summary>
        /// Executes the connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">func</exception>
        public T ExecuteConn<T>(Func<TConn, T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var conn = Connection;

            var initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    conn.Open(); // open connection if not already open

                return func(conn);
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    conn.Close(); // only close connection if not initially open
            }
        }

        /// <summary>
        /// Executes the proc.
        /// </summary>
        /// <param name="proc">The proc.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="setParameters">The set parameters.</param>
        /// <param name="readOutParams">The read out parameters.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public int ExecuteProc(
            string proc,
            int? commandTimeout = null,
            Action<TParColl> setParameters = null,
            Action<TCmd> readOutParams = null,
            params TPar[] parameters)
        {
            return ExecuteCmd(
                proc,
                CommandType.StoredProcedure,
                commandTimeout,
                setParameters,
                parameters,
                (conn, cmd) => {
                    var res = cmd.ExecuteNonQuery();
                    readOutParams?.Invoke(cmd);
                    return res;
                });
        }

        /// <summary>
        /// Executes the proc reader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proc">The proc.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="setParameters">The set parameters.</param>
        /// <param name="readOutParams">The read out parameters.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="convertAll">The convert all.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">converter</exception>
        public IEnumerable<T> ExecuteProcReader<T>(
            string proc,
            int? commandTimeout = null,
            Action<TParColl> setParameters = null,
            Action<TCmd> readOutParams = null,
            Func<TReader, T> converter = null,
            Func<IEnumerable<TReader>, IEnumerable<T>> convertAll = null,
            params TPar[] parameters)
        {
            if (converter == null && convertAll == null)
                throw new ArgumentNullException(nameof(converter));

            return ExecuteCmd(
                proc,
                CommandType.StoredProcedure,
                commandTimeout,
                setParameters,
                parameters,
                (conn, cmd) => {
                    if (convertAll != null)
                        return convertAll(ExecuteReader(conn, cmd));

                    Debug.Assert(converter != null, "converter != null");
                    return ExecuteReader(conn, cmd).Select(converter).ToList();
                });
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="cmd">The command.</param>
        /// <returns></returns>
        public IEnumerable<TReader> ExecuteReader(TConn conn, TCmd cmd)
        {
            var d = DateTimeOffset.Now;
            using (var reader = cmd.ExecuteReader())
            {
                var w = DateTimeOffset.Now.Subtract(d);
                var sd = w;
                while (reader.Read())
                {
                    yield return (TReader)reader;
                }
            }
        }

        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public int ExecuteSql(string sql)
        {
            return ExecuteSql(sql, null);
        }

        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="setParameters">The set parameters.</param>
        /// <param name="readOutParams">The read out parameters.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public int ExecuteSql(
            string sql,
            int? commandTimeout = null,
            Action<TParColl> setParameters = null,
            Action<TCmd> readOutParams = null,
            params TPar[] parameters)
        {
            return ExecuteCmd(
                sql,
                CommandType.Text,
                commandTimeout,
                setParameters,
                parameters,
                (conn, cmd) => {
                    var res = cmd.ExecuteNonQuery();
                    readOutParams?.Invoke(cmd);
                    return res;
                });
        }

        /// <summary>
        /// Executes the SQL reader.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="setParameters">The set parameters.</param>
        /// <param name="readOutParams">The read out parameters.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public IEnumerable<TReader> ExecuteSqlReader(
            string sql,
            int? commandTimeout = null,
            Action<TParColl> setParameters = null,
            Action<TCmd> readOutParams = null,
            params TPar[] parameters)
        {
            var conn = Connection;

            var initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    conn.Open(); // open connection if not already open

                using (var cmd = (TCmd)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    if (DbContext.Database.CurrentTransaction != null)
                        cmd.Transaction = DbContext.Database.CurrentTransaction.UnderlyingTransaction;

                    setParameters?.Invoke((TParColl)cmd.Parameters);

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters.Where(_ => _ != null).ToArray());

                    if (commandTimeout != null)
                        cmd.CommandTimeout = commandTimeout.Value;

                    using (var reader = (TReader)cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            yield return reader;
                        }
                }
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    conn.Close(); // only close connection if not initially open
            }
        }

        /// <summary>
        /// Executes the SQL reader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="setParameters">The set parameters.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="convertAll">The convert all.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteSqlReader<T>(
            string sql,
            int? commandTimeout = null,
            Action<TParColl> setParameters = null,
            Func<TReader, T> converter = null,
            Func<IEnumerable<TReader>, IEnumerable<T>> convertAll = null,
            params TPar[] parameters)
        {
            return ExecuteCmd(
                sql,
                CommandType.Text,
                commandTimeout,
                setParameters,
                parameters,
                (conn, cmd) => {
                    if (convertAll != null)
                        return convertAll(ExecuteReader(conn, cmd));

                    Debug.Assert(converter != null, "converter != null");
                    return ExecuteReader(conn, cmd).Select(converter).ToList();
                });
        }
    }
}