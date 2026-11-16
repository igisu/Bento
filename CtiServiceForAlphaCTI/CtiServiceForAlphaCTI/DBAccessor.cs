using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace com.emukikaku.CtiServiceAlphaCTI
{
    public class DBAccessor
    {
        #region member
        private string _server;
        private string _userId;
        private string _password;
        private string _databaseName;
        private string _connectionString;

        private SqlConnection _conn;  
        #endregion

        #region Constructor
        public DBAccessor(string server ,string userId ,string password ,string databaseName)
        {
            _server = server;
            _userId = userId;
            _password = password;
            _databaseName = databaseName;
            _connectionString = createConnectionString(server ,userId ,password ,databaseName);

            _conn = null;
        }
        #endregion

        #region Create ConnectionString
        private string createConnectionString(string server, string userId, string password, string databaseName)
        {
            return 
                string.Format(
                    "data source={0};database={1};password={2};user id={3};workstation id=localhost;packet size=4096"
                    ,server,databaseName,password,userId);
        }
        #endregion

        #region Open
        public void open()
        {
            if (_conn == null)
            {
                _conn = new SqlConnection(_connectionString);
            }
            _conn.Open();
        }
        #endregion

        #region Close
        public void close()
        {
            _conn.Close();
        }
        #endregion

        #region Fill
        public DataTable fill(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql,this._conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        #endregion
    }
}
