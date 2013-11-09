using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace Zeus.Server.Library.Database.Mysql {

    /// <summary>
    /// Generic class to access and use a mysql connection.
    /// </summary>
    public class Provider : IProvider {

        protected string _connectionString;


        /// <summary>
        /// Gets the <see cref="ConnectionState"/> state
        /// </summary>
        public ConnectionState ConnectionState {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the last thrown exception
        /// </summary>
        public Exception LastException {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the <see cref="DbConnection"/> object
        /// </summary>
        public DbConnection Connection {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the last <see cref="DbCommand"/> object
        /// </summary>
        public DbCommand Command {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the last <see cref="DbDataAdapter"/> object
        /// </summary>
        public DbDataAdapter Adapter {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the last <see cref="DbDataReader"/> object
        /// </summary>
        public DbDataReader Reader {
            get;
            protected set;
        }

        /// <summary>
        /// Gets true or false if successfull connected
        /// </summary>
        public bool IsConnected {
            get { return Connection != null && Connection.State == System.Data.ConnectionState.Open; }
        }


        public Provider() {
            ConnectionState = ConnectionState.Closed;
        }

        public Provider(string connectionString)
            : this() {
            _connectionString = connectionString;
        }

        ~Provider() {
            Dispose();
        }


        #region Prepare(), Open(), Close()
        /// <summary>
        /// Tests the connection details and try to open a <see cref="MySqlConnection"/>
        /// <para>If opened successfully, its closed afterwards</para>
        /// <para>Also, if opened, the returned value will be the return from <see cref="Close"/> method</para>
        /// </summary>
        public ConnectionError Prepare() {
            if (string.IsNullOrEmpty(_connectionString)) {
                throw new ArgumentNullException("Connect-string cant be empty");
            }

            Connection = new MySqlConnection(_connectionString);
            var result = Open();
            if (result == ConnectionError.None) {
                result = Close(false);
            }

            return result;
        }

        /// <summary>
        /// Opens the <see cref="MySqlConnection"/>
        /// <para>Warning: You dont need this! Just test the connection with <see cref="Prepare"/> and use the Query* Methods</para>
        /// <para>The connection will be opened and closed automatically</para>
        /// </summary>
        /// <returns></returns>
        public ConnectionError Open() {
            LastException = null;

            // Create the connection, if needed
            if (Connection == null) {
                var prepareResult = Prepare();
                if (prepareResult != ConnectionError.None) {
                    return prepareResult;
                }

                if (Connection == null) {
                    throw new ApplicationException("Unable to create mysql connection - unidentified exception, please report this");
                }
            }

            // Already opened?
            if (Connection.State == System.Data.ConnectionState.Open) {
                ConnectionState = ConnectionState.Open;
                return ConnectionError.None;
            }

            try {
                Connection.Open();

                if (Connection.State != System.Data.ConnectionState.Open) {
                    ConnectionState = ConnectionState.Error;
                    return ConnectionError.FailedToOpen;
                }
            } catch (MySqlException e) {
                ConnectionState = ConnectionState.Error;
                LastException = e;
                return ConnectionError.CanNotConnectToDb;
            } catch (Exception e) {
                ConnectionState = ConnectionState.Error;
                LastException = e;
                return ConnectionError.UnknownOpen;
            }

            ConnectionState = ConnectionState.Open;
            return ConnectionError.None;
        }

        /// <summary>
        /// Closes an instanced connection - will be called automatically on dispose
        /// </summary>
        /// <param name="freeConnection"></param>
        /// <returns></returns>
        public ConnectionError Close(bool freeConnection) {
            LastException = null;

            if (Connection == null) {
                ConnectionState = ConnectionState.Error;
                return ConnectionError.CloseBeforeInit;
            }

            try {
                Connection.Close();

                if (Connection.State != System.Data.ConnectionState.Closed) {
                    ConnectionState = ConnectionState.Error;
                    return ConnectionError.FailedToClose;
                }
            } catch (MySqlException e) {
                ConnectionState = ConnectionState.Error;
                LastException = e;
                return ConnectionError.CanNotDisconnectFromDb;
            } catch (Exception e) {
                ConnectionState = ConnectionState.Error;
                LastException = e;
                return ConnectionError.UnknownClose;
            } finally {
                // free Memory
                if (Command != null)
                    Command.Dispose();
                if (Adapter != null)
                    Adapter.Dispose();
                if (freeConnection && Connection != null)
                    Connection.Dispose();
            }

            ConnectionState = ConnectionState.Closed;
            return ConnectionError.None;
        }
        #endregion


        /// <summary>
        /// Returns the MySQL LAST_INSERT_ID() Statement
        /// </summary>
        /// <returns></returns>
        public virtual int GetLastInsertId() {
            var table = Query("SELECT LAST_INSERT_ID() AS id");
            if (table.Rows.Count == 0) {
                return -1;
            }

            return table.Rows[0].Field<int>("id");
        }

        #region Query Methods
        /// <summary>
        /// Executes a Query and returns the amount of affected rows
        /// </summary>
        /// <param name="query"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public virtual int QuerySimple(string query, params object[] arg) {
            LastException = null;

            query = string.Format(query, arg);

            var result = 0;
            if (query.Length == 0) {
                return result;
            }

            try {
                Open();
                Command = Connection.CreateCommand();
                Command.CommandText = query;

                result = Command.ExecuteNonQuery();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
                LastException = ex;
                return 0;
            }

            return result;
        }

        /// <summary>
        /// Loads all text from the given file and tries to execute it.
        /// <para>Used <see cref="Encoding.Default"/></para>
        /// <para>Used <see cref="QuerySimple"/>, so the return will be the amount of affected rows</para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Amount of affected rows</returns>
        public virtual int QueryFromFile(string filePath) {
            return QueryFromFile(filePath, Encoding.Default);
        }

        /// <summary>
        /// Loads all text from the given file and tries to execute it.
        /// <para>Used <see cref="QuerySimple"/>, so the return will be the amount of affected rows</para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enc">The used encoding to read this file</param>
        /// <returns>Amount of affected rows</returns>
        public virtual int QueryFromFile(string filePath, Encoding enc) {
            return QuerySimple(File.ReadAllText(filePath, enc));
        }

        /// <summary>
        /// Executes a query and reads and returns the first <see cref="Int32"/> value
        /// </summary>
        /// <param name="query"></param>
        /// <param name="arg"></param>
        /// <returns>True, if the first <see cref="Int32"/> value is above zero</returns>
        public virtual bool QueryBool(string query, params object[] arg) {
            query = string.Format(query, arg);

            var result = false;
            LastException = null;
            if (query.Length == 0) {
                return false;
            }

            try {
                Open();
                Command = Connection.CreateCommand();
                Command.CommandText = query;

                Reader = Command.ExecuteReader();
                while (Reader.Read()) {
                    result = (Reader.GetInt32(0) > 0);
                }
                Reader.Close();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
                LastException = ex;
                return false;
            }

            return result;
        }

        /// <summary>
        /// Executes a query and returns the result as a <see cref="DataTable"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="arg"></param>
        /// <returns>
        /// DataTable with all results
        /// <para>Note: the DataTable will be empty if an error occours</para>
        /// </returns>
        public virtual DataTable Query(string query, params object[] arg) {
            query = string.Format(query, arg);

            var result = new DataTable();
            LastException = null;
            if (query.Length == 0) {
                return result;
            }

            Open();

            Adapter = new MySqlDataAdapter {
                SelectCommand = new MySqlCommand(query, Connection as MySqlConnection)
            };
            try {
                Adapter.Fill(result);
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
                LastException = ex;
                return new DataTable();
            }

            return result;
        }
        #endregion


        /// <summary>
        /// [DEBUG] Prints all colums and data-types of the given table
        /// </summary>
        /// <param name="table"></param>
        public static void PrintTableTypes(DataTable table) {
            for (var i = 0; i < table.Columns.Count; i++) {
                Console.WriteLine("{0}: {1}", table.Columns[i].ColumnName, table.Columns[i].DataType);
            }
        }


        /// <summary>
        /// Closes the connection and free's all ressources
        /// </summary>
        public virtual void Dispose() {
            Close(true);
        }

        /// <summary>
        /// Closes the connection and free's all ressources
        /// </summary>
        void IDisposable.Dispose() {
            Dispose();
        }

    }

}
