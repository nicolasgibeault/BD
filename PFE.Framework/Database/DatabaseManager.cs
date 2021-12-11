using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace PFE.Framework.Database
{
    public class DatabaseManager : IDisposable
    {
        private static readonly DatabaseManager instance = new DatabaseManager();
        private SqliteConnection _connection;
        public SqliteConnection _getConnection
        {
            get 
            {
                if (_connection == null || _connection.State == System.Data.ConnectionState.Closed || _connection.State == System.Data.ConnectionState.Broken)
                {
                    _init();
                }

                return _connection;
            }
        }
        private void _init()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            connectionStringBuilder.DataSource = "./SqliteDB.db";

            _connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            _connection.Open();

            // Init database table creation for POC
            var createTableCmd = _connection.CreateCommand();
            createTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS datatable(id integer primary key auto increment, connectiondate text, connectiontime text, connection integer)";
            createTableCmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        private DatabaseManager()
        {

        }

        public static DatabaseManager Instance
        {
            get
            {
                return instance;
            }
        }


    }
}
