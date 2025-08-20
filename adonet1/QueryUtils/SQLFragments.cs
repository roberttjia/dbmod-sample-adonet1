using System;
using System.Collections.Generic;

namespace adonet1.QueryUtils
{
    /// <summary>
    /// SQL fragments stored separately from main queries - challenging for detection tools
    /// </summary>
    public static class SQLFragments
    {
        // Base table references
        public const string CUSTOMERS_TABLE = "tbl_customers";
        public const string PRODUCTS_TABLE = "tbl_products";
        public const string ORDERS_TABLE = "tbl_orders";

        // Column references scattered across constants
        public const string CUST_ID_COL = "cust_id";
        public const string FIRST_NAME_COL = "first_nm";
        public const string LAST_NAME_COL = "last_nm";
        public const string PHONE_COL = "phone";
        public const string PRODUCT_ID_COL = "product_id";
        public const string ORDER_ID_COL = "order_id";

        // Common WHERE clause fragments
        public static readonly Dictionary<string, string> WhereFragments = new Dictionary<string, string>
        {
            ["ActiveCustomers"] = "WHERE active = 1",
            ["RecentOrders"] = "WHERE order_date > DATEADD(day, -30, GETDATE())",
            ["ExpensiveProducts"] = "WHERE price > 100.00"
        };

        // JOIN fragments
        public static string GetCustomerOrderJoin()
        {
            return $"INNER JOIN {ORDERS_TABLE} o ON c.{CUST_ID_COL} = o.{CUST_ID_COL}";
        }

        public static string GetOrderProductJoin()
        {
            return $"INNER JOIN {PRODUCTS_TABLE} p ON o.{PRODUCT_ID_COL} = p.{PRODUCT_ID_COL}";
        }

        // SELECT fragments
        public static string GetCustomerSelectList()
        {
            return $"c.{CUST_ID_COL}, c.{FIRST_NAME_COL}, c.{LAST_NAME_COL}, c.{PHONE_COL}";
        }

        public static string GetProductSelectList()
        {
            return $"p.{PRODUCT_ID_COL}, p.name, p.code, p.description, p.price";
        }
    }
}
