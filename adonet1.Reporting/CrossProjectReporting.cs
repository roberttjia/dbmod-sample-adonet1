using System;
using System.Data;
using Microsoft.Data.SqlClient;
using adonet1.QueryUtils; // Cross-project reference

namespace adonet1.Reporting
{
    /// <summary>
    /// Tests cross-project database object detection
    /// </summary>
    public class CrossProjectReporting
    {
        private readonly string connectionString;

        public CrossProjectReporting(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Uses table constants from main project
        public DataTable GetCustomerSummary()
        {
            var sql = $@"
                SELECT 
                    COUNT(*) as TotalCustomers,
                    COUNT(DISTINCT SUBSTRING({SQLFragments.LAST_NAME_COL}, 1, 1)) as UniqueLastNameInitials
                FROM {SQLFragments.CUSTOMERS_TABLE}";

            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                var adapter = new SqlDataAdapter(sql, connection);
                adapter.Fill(dt);
            }
            return dt;
        }

        // Complex query spanning multiple tables with fragments from main project
        public DataTable GetSalesReport()
        {
            var sql = $@"
                SELECT 
                    c.{SQLFragments.FIRST_NAME_COL} + ' ' + c.{SQLFragments.LAST_NAME_COL} as CustomerName,
                    p.name as ProductName,
                    o.quantity,
                    o.total_price,
                    RANK() OVER (ORDER BY o.total_price DESC) as SalesRank
                FROM {SQLFragments.CUSTOMERS_TABLE} c
                {SQLFragments.GetCustomerOrderJoin()}
                {SQLFragments.GetOrderProductJoin()}
                WHERE o.total_price > 50.00
                ORDER BY o.total_price DESC";

            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                var adapter = new SqlDataAdapter(sql, connection);
                adapter.Fill(dt);
            }
            return dt;
        }

        // Uses stored procedure name that might be in different schema
        public int ArchiveOldOrders(DateTime cutoffDate)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("reporting.proc_ArchiveOrders", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CutoffDate", cutoffDate);
                command.Parameters.Add("@ArchivedCount", SqlDbType.Int).Direction = ParameterDirection.Output;

                connection.Open();
                command.ExecuteNonQuery();
                return (int)command.Parameters["@ArchivedCount"].Value;
            }
        }
    }
}
