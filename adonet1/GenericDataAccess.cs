using adonet1.models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace adonet1
{
    /// <summary>
    /// Generic data access class pattern. Common DB related functionality is encaptulated here
    /// 
    /// 11c Generic Data Access: Reusable data access patterns
    /// </summary>
    public class GenericDataAccess
    {
        string _connectionString;
        public GenericDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Generic method to execute an SQL statement that returns a DataSet.
        /// Only input parameters.
        /// 
        /// 11a Generic Data Access: Parameterized generic methods for any SQL
        /// 11b Generic Data Access: Generic parameter collections
        /// </summary>
        /// <param name="SelectSQL"></param>
        /// <param name="sqlParams"></param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteSelectSQL(string SelectSQL, GenericSQLParams sqlParams)
        {
            var dataSet = new DataSet();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(SelectSQL, connection);
                foreach (var param in sqlParams.Keys)
                {
                    command.Parameters.AddWithValue(param, sqlParams[param]);
                }
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(dataSet);
            }
            return dataSet;
        }

        /// <summary>
        /// Execute a stored procedure that returns a DataSet
        /// 
        /// 5a  Stored Procedures: CommandType.StoredProcedure
        /// 11a Generic Data Access: Parameterized generic methods for any SQL
        /// 11b Generic Data Access: Generic parameter collections
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public DataSet ExecuteSelectSP(string storedProcedure, GenericSQLParams sqlParams)
        {
            var dataSet = new DataSet();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(storedProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;

                foreach (var param in sqlParams.Keys)
                {
                    command.Parameters.AddWithValue(param, sqlParams[param]);
                }
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(dataSet);
            }
            return dataSet;
        }

        /// <summary>
        /// Execute any SQL statement using ExecuteNonQuery
        /// 
        /// 11a Generic Data Access: Parameterized generic methods for any SQL
        /// 11b Generic Data Access: Generic parameter collections
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public int ExecuteNonQuerySQL(string sql, GenericSQLParams sqlParams)
        {
            int numRowsAffected = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(sql, connection);
                foreach (var param in sqlParams.Keys)
                {
                    command.Parameters.AddWithValue(param, sqlParams[param]);
                }
                connection.Open();
                numRowsAffected = command.ExecuteNonQuery();
            }
            return numRowsAffected;
        }

        /// <summary>
        /// Execute any stored procedure using ExecuteNonQuery
        /// 
        /// 5a  Stored Procedures: CommandType.StoredProcedure
        /// 11b Generic Data Access: Generic parameter collections
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public int ExecuteNonQuerySP(string storedProcedure, GenericSQLParams sqlParams)
        {
            int numRowsAffected = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(storedProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;

                foreach (var param in sqlParams.Keys)
                {
                    command.Parameters.AddWithValue(param, sqlParams[param]);
                }
                connection.Open();

                // This will return numrows affected for the last CRUD
                numRowsAffected = command.ExecuteNonQuery();
            }
            return numRowsAffected;
        }
    }
}
