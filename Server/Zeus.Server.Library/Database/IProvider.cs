using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Zeus.Server.Library.Database {

    public interface IProvider : IDisposable {

        /// <summary>
        /// Gets the <see cref="ConnectionState"/> state
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        /// Gets the last thrown exception
        /// </summary>
        Exception LastException { get; }

        /// <summary>
        /// Gets the <see cref="DbConnection"/> object
        /// </summary>
        DbConnection Connection { get; }

        /// <summary>
        /// Gets the last <see cref="DbCommand"/> object
        /// </summary>
        DbCommand Command { get; }

        /// <summary>
        /// Gets the last <see cref="DbDataAdapter"/> object
        /// </summary>
        DbDataAdapter Adapter { get; }

        /// <summary>
        /// Gets true or false if successfull connected
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Tests the connection details and try to open a <see cref="DbConnection"/>
        /// <para>If opened successfully, its closed afterwards</para>
        /// <para>Also, if opened, the returned value will be the return from <see cref="Close"/> method</para>
        /// </summary>
        ConnectionError Prepare();

        /// <summary>
        /// Opens the <see cref="DbConnection"/>
        /// <para>Warning: You dont need this! Just test the connection with <see cref="Prepare"/> and use the Query* Methods</para>
        /// <para>The connection will be opened and closed automatically</para>
        /// </summary>
        /// <returns></returns>
        ConnectionError Open();

        /// <summary>
        /// Closes an instanced connection - will be called automatically on dispose
        /// </summary>
        /// <param name="freeConnection"></param>
        /// <returns></returns>
        ConnectionError Close(bool freeConnection);

        /// <summary>
        /// Returns the MySQL LAST_INSERT_ID() Statement
        /// </summary>
        /// <returns></returns>
        int GetLastInsertId();

        /// <summary>
        /// Executes a Query and returns the amount of affected rows
        /// </summary>
        /// <param name="query"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        int QuerySimple(string query, params object[] arg);

        /// <summary>
        /// Loads all text from the given file and tries to execute it.
        /// <para>Used <see cref="Encoding.Default"/></para>
        /// <para>Used <see cref="QuerySimple"/>, so the return will be the amount of affected rows</para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Amount of affected rows</returns>
        int QueryFromFile(string filePath);

        /// <summary>
        /// Loads all text from the given file and tries to execute it.
        /// <para>Used <see cref="QuerySimple"/>, so the return will be the amount of affected rows</para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enc">The used encoding to read this file</param>
        /// <returns>Amount of affected rows</returns>
        int QueryFromFile(string filePath, Encoding enc);

        /// <summary>
        /// Executes a query and reads and returns the first <see cref="Int32"/> value
        /// </summary>
        /// <param name="query"></param>
        /// <param name="arg"></param>
        /// <returns>True, if the first <see cref="Int32"/> value is above zero</returns>
        bool QueryBool(string query, params object[] arg);

        /// <summary>
        /// Executes a query and returns the result as a <see cref="DataTable"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="arg"></param>
        /// <returns>
        /// DataTable with all results
        /// <para>Note: the DataTable will be empty if an error occours</para>
        /// </returns>
        DataTable Query(string query, params object[] arg);
    }

}