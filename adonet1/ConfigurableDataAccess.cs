using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using adonet1.QueryUtils;

namespace adonet1
{
    /// <summary>
    /// Uses configuration and external sources for SQL - challenging for static analysis
    /// </summary>
    public class ConfigurableDataAccess
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;
        private readonly FragmentedQueryBuilder queryBuilder;

        public ConfigurableDataAccess()
        {
            configuration = ConfigHelper.GetConfiguration();
            connectionString = configuration.GetConnectionString("DefaultConnection");
            queryBuilder = new FragmentedQueryBuilder();
        }

        // Pattern: SQL from configuration (would need appsettings.json entries)
        public DataTable ExecuteConfiguredQuery(string queryKey)
        {
            var sql = configuration[$"CustomQueries:{queryKey}"] ?? 
                     "SELECT 'Query not found' as Message";
            
            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                var adapter = new SqlDataAdapter(sql, connection);
                adapter.Fill(dt);
            }
            return dt;
        }

        // Pattern: SQL built from fragments in different classes
        public DataTable GetCustomerReport(bool includeOrders, bool includeProducts)
        {
            var sql = queryBuilder.BuildCustomerReportQuery(includeOrders, includeProducts);
            
            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                var adapter = new SqlDataAdapter(sql, connection);
                adapter.Fill(dt);
            }
            return dt;
        }

        // Pattern: SQL with runtime table name resolution
        public int GetTableRecordCount(string tableType)
        {
            var sql = queryBuilder.GetTableStatsQuery(tableType);
            
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(sql, connection);
                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }

        // Pattern: SQL with method chaining and fluent interface
        public DataTable ExecuteFluentQuery()
        {
            var sql = new StringBuilder()
                .AppendSelect()
                .AppendFrom()
                .AppendWhere()
                .ToString();
            
            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                var adapter = new SqlDataAdapter(sql, connection);
                adapter.Fill(dt);
            }
            return dt;
        }
    }

    // Extension methods for fluent SQL building - more fragmentation
    public static class SQLBuilderExtensions
    {
        public static StringBuilder AppendSelect(this StringBuilder sb)
        {
            return sb.Append($"SELECT {SQLFragments.GetCustomerSelectList()}");
        }

        public static StringBuilder AppendFrom(this StringBuilder sb)
        {
            return sb.Append($" FROM [dbo].{SQLFragments.CUSTOMERS_TABLE} c");
        }

        public static StringBuilder AppendWhere(this StringBuilder sb)
        {
            return sb.Append($" WHERE c.{SQLFragments.CUST_ID_COL} > 0");
        }
    }
}
