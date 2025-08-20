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

        /// <summary>
        /// Pattern: SQL from configuration (would need appsettings.json entries)
        /// 
        /// 9a Configuration-Driven SQL: SQL queries from appsettings.json
        /// </summary>
        /// <param name="queryKey"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Pattern: SQL built from fragments in different classes
        /// 
        /// 6d Dynamic SQL Construction: SQL fragments stored in arrays/collections
        /// 7a Fragmented SQL References: Table/column names in constants
        /// 7b Fragmented SQL References: SQL parts scattered across multiple classes
        /// 7c Fragmented SQL References: JOIN fragments as separate methods
        /// </summary>
        /// <param name="includeOrders"></param>
        /// <param name="includeProducts"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Pattern: SQL with runtime table name resolution
        /// 
        /// 12d Complex Query Patterns: Computed table names from variables
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Pattern: SQL with method chaining and fluent interface
        /// 
        /// 6b Dynamic SQL Construction: StringBuilder for complex query building
        /// 7a Fragmented SQL References: Table/column names in constants
        /// 7b Fragmented SQL References: SQL parts scattered across multiple classes
        /// 9c Configuration-Driven SQL: Method chaining/fluent interfaces for SQL building
        /// </summary>
        /// <returns></returns>
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

}
