using System;
using System.Data;
using Microsoft.Data.SqlClient;
using adonet1.models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace adonet1
{
    /// <summary>
    /// Advanced patterns that challenge database object detection tools
    /// </summary>
    public class AdvancedDataAccess
    {
        private readonly string connectionString = ConfigHelper.GetConfiguration().GetConnectionString("DefaultConnection");

        // Pattern: Dynamic table names from variables
        public DataTable GetDataFromDynamicTable(string tableName, string columnName)
        {
            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                // Tool must detect table/column references from variables
                var sql = $"SELECT {columnName} FROM {tableName} WHERE 1 = 1";
                var adapter = new SqlDataAdapter(sql, connection);
                adapter.Fill(dt);
            }
            return dt;
        }

        // Pattern: SQL built across multiple string operations
        public List<Customer> GetCustomersWithComplexQuery(string searchTerm)
        {
            var customers = new List<Customer>();
            var baseQuery = "SELECT c.cust_id, c.first_nm, c.last_nm";
            var fromClause = " FROM tbl_customers c";
            var joinClause = " LEFT JOIN tbl_orders o ON c.cust_id = o.cust_id";
            var whereClause = " WHERE c.last_nm LIKE @SearchTerm";
            var orderClause = " ORDER BY c.last_nm";
            
            var fullQuery = baseQuery + fromClause + joinClause + whereClause + orderClause;
            
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(fullQuery, connection);
                command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            CustomerId = reader.GetInt32("cust_id"),
                            FirstName = reader.GetString("first_nm"),
                            LastName = reader.GetString("last_nm")
                        });
                    }
                }
            }
            return customers;
        }

        // Pattern: Conditional SQL construction
        public DataTable SearchWithDynamicConditions(bool includePhone, bool includeOrders)
        {
            var selectPart = "SELECT c.cust_id, c.first_nm, c.last_nm";
            if (includePhone) selectPart += ", c.phone";
            
            var fromPart = " FROM tbl_customers c";
            if (includeOrders) fromPart += " INNER JOIN tbl_orders o ON c.cust_id = o.cust_id";
            
            var sql = selectPart + fromPart + " WHERE c.cust_id > 0";
            
            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                var adapter = new SqlDataAdapter(sql, connection);
                adapter.Fill(dt);
            }
            return dt;
        }

        // Pattern: SQL in string arrays/collections
        private readonly string[] reportQueries = {
            "SELECT COUNT(*) FROM tbl_customers",
            "SELECT COUNT(*) FROM tbl_products WHERE price > 100",
            "SELECT SUM(total_price) FROM tbl_orders"
        };

        public Dictionary<string, object> RunReports()
        {
            var results = new Dictionary<string, object>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                for (int i = 0; i < reportQueries.Length; i++)
                {
                    var command = new SqlCommand(reportQueries[i], connection);
                    results[$"Report{i}"] = command.ExecuteScalar();
                }
            }
            return results;
        }

        // Pattern: SQL with embedded subqueries
        public List<Customer> GetTopCustomersByOrderValue()
        {
            var customers = new List<Customer>();
            var sql = @"
                SELECT c.cust_id, c.first_nm, c.last_nm, c.phone
                FROM tbl_customers c
                WHERE c.cust_id IN (
                    SELECT TOP 5 o.cust_id 
                    FROM tbl_orders o 
                    GROUP BY o.cust_id 
                    ORDER BY SUM(o.total_price) DESC
                )";
            
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(sql, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            CustomerId = reader.GetInt32("cust_id"),
                            FirstName = reader.GetString("first_nm"),
                            LastName = reader.GetString("last_nm"),
                            Phone = reader.GetString("phone")
                        });
                    }
                }
            }
            return customers;
        }
    }
}
