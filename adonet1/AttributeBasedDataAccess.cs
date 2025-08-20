using System;
using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace adonet1
{
    /// <summary>
    /// SQL stored in attributes - very challenging for detection tools
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SqlQueryAttribute : Attribute
    {
        public string Query { get; }
        public SqlQueryAttribute(string query) => Query = query;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class StoredProcAttribute : Attribute
    {
        public string ProcName { get; }
        public StoredProcAttribute(string procName) => ProcName = procName;
    }

    public class AttributeBasedDataAccess
    {
        private readonly string connectionString;

        public AttributeBasedDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        [SqlQuery("SELECT cust_id, first_nm, last_nm, phone FROM tbl_customers WHERE cust_id = @CustomerId")]
        public DataTable GetCustomerByIdFromAttribute(int customerId)
        {
            var method = MethodBase.GetCurrentMethod();
            var attr = method.GetCustomAttribute<SqlQueryAttribute>();
            
            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(attr.Query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
            }
            return dt;
        }

        [SqlQuery(@"
            SELECT 
                c.cust_id,
                c.first_nm + ' ' + c.last_nm as FullName,
                COUNT(o.order_id) as OrderCount,
                ISNULL(SUM(o.total_price), 0) as TotalSpent
            FROM tbl_customers c
            LEFT JOIN tbl_orders o ON c.cust_id = o.cust_id
            WHERE c.cust_id = @CustomerId
            GROUP BY c.cust_id, c.first_nm, c.last_nm")]
        public DataTable GetCustomerSummaryFromAttribute(int customerId)
        {
            return ExecuteAttributeQuery(customerId);
        }

        [StoredProc("proc_SearchCustomersByLastName")]
        public DataTable SearchCustomersFromAttribute(string lastName, int maxRows)
        {
            var method = MethodBase.GetCurrentMethod();
            var attr = method.GetCustomAttribute<StoredProcAttribute>();
            
            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(attr.ProcName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@LastNameSearch", lastName);
                command.Parameters.AddWithValue("@MaxRows", maxRows);
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
            }
            return dt;
        }

        private DataTable ExecuteAttributeQuery(params object[] parameters)
        {
            var method = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
            var attr = method.GetCustomAttribute<SqlQueryAttribute>();
            
            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(attr.Query, connection);
                // Simplified parameter handling for demo
                if (parameters.Length > 0)
                {
                    command.Parameters.AddWithValue("@CustomerId", parameters[0]);
                }
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
            }
            return dt;
        }
    }

    /// <summary>
    /// SQL queries stored in static readonly fields - another challenging pattern
    /// </summary>
    public static class QueryRepository
    {
        public static readonly string GET_CUSTOMER_ORDERS = @"
            SELECT 
                o.order_id,
                o.quantity,
                o.total_price,
                p.name as product_name,
                p.code as product_code
            FROM tbl_orders o
            INNER JOIN tbl_products p ON o.product_id = p.product_id
            WHERE o.cust_id = @CustomerId
            ORDER BY o.order_id DESC";

        public static readonly string UPDATE_CUSTOMER_STATUS = @"
            UPDATE tbl_customers 
            SET status = @Status, 
                last_updated = GETDATE() 
            WHERE cust_id = @CustomerId";

        // SQL with computed table names
        public static string GetTableCountQuery(string tablePrefix)
        {
            return $"SELECT COUNT(*) FROM {tablePrefix}_customers UNION ALL " +
                   $"SELECT COUNT(*) FROM {tablePrefix}_products UNION ALL " +
                   $"SELECT COUNT(*) FROM {tablePrefix}_orders";
        }
    }
}
