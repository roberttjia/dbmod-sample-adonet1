using System;
using System.Text;

namespace adonet1.QueryUtils
{
    /// <summary>
    /// Builds queries using fragments from different sources - tests cross-file detection
    /// </summary>
    public class FragmentedQueryBuilder
    {
        // More table references in different format
        private const string SCHEMA_PREFIX = "[dbo].";
        
        public string BuildCustomerReportQuery(bool includeOrders, bool includeProducts)
        {
            var query = new StringBuilder();
            
            // SELECT using fragments from SQLFragments class
            query.Append($"SELECT {SQLFragments.GetCustomerSelectList()}");
            
            if (includeOrders)
            {
                query.Append($", o.{SQLFragments.ORDER_ID_COL}, o.quantity, o.total_price");
            }
            
            if (includeProducts)
            {
                query.Append($", {SQLFragments.GetProductSelectList()}");
            }
            
            // FROM using constant with schema
            query.Append($" FROM {SCHEMA_PREFIX}{SQLFragments.CUSTOMERS_TABLE} c");
            
            // JOINs using fragment methods
            if (includeOrders)
            {
                query.Append($" {SQLFragments.GetCustomerOrderJoin()}");
            }
            
            if (includeProducts && includeOrders)
            {
                query.Append($" {SQLFragments.GetOrderProductJoin()}");
            }
            
            return query.ToString();
        }

        public string BuildDynamicFilterQuery(string filterType)
        {
            var baseQuery = $"SELECT * FROM {SCHEMA_PREFIX}{SQLFragments.CUSTOMERS_TABLE} c ";
            
            if (SQLFragments.WhereFragments.ContainsKey(filterType))
            {
                baseQuery += SQLFragments.WhereFragments[filterType];
            }
            
            return baseQuery;
        }

        // Method that returns SQL with table names computed at runtime
        public string GetTableStatsQuery(string tableType)
        {
            string tableName = tableType.ToLower() switch
            {
                "customers" => SQLFragments.CUSTOMERS_TABLE,
                "products" => SQLFragments.PRODUCTS_TABLE,
                "orders" => SQLFragments.ORDERS_TABLE,
                _ => throw new ArgumentException("Invalid table type")
            };
            
            return $"SELECT COUNT(*) as RecordCount FROM {SCHEMA_PREFIX}{tableName}";
        }
    }
}
