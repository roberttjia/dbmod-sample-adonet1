using System;
using System.Data;
using Microsoft.Data.SqlClient;
using adonet1.models;
using adonet1.QueryUtils;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace adonet1
{
    /// <summary>
    /// Customer Data Access class
    /// </summary>
    public class CustomerDataAccess
    {
        private readonly string connectionString = ConfigHelper.GetConfiguration().GetConnectionString("DefaultConnection");

        /// <summary>
        /// This is a common access pattern to insert a row into the DB.
        /// - INSERT
        /// - parameterized SQL
        /// - SQL string concatenation "+"
        /// </summary>
        /// <param name="cust"></param>
        /// <returns></returns>
        public Customer InsertCustomerRecord(Customer cust)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(
                    "INSERT INTO [dbo].[tbl_customers] (first_nm, last_nm, phone)" +
                    " OUTPUT INSERTED.cust_id" +
                    " VALUES (@FirstName, @LastName, @Phone)",
                    connection);
                command.Parameters.AddWithValue("@FirstName", cust.FirstName);
                command.Parameters.AddWithValue("@LastName", cust.LastName);
                command.Parameters.AddWithValue("@Phone", cust.Phone);
                
                connection.Open();
                cust.CustomerId = (int)command.ExecuteScalar();
            }
            return cust;
        }

        /// <summary>
        /// Update customer record using parameterized SQL. 
        /// - UPDATE
        /// - parameterized SQL
        /// - SQL string concatenation "+"
        /// </summary>
        /// <param name="cust"></param>
        /// <returns></returns>
        public int UpdateCustomerRecord(Customer cust)
        {
            int numRowsAffected = 0;
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(
                    "UPDATE [dbo].[tbl_customers] " +
                    "SET first_nm = @FirstName, " +
                    "    last_nm = @LastName, " +
                    "    phone = @Phone " +
                    "WHERE cust_id = @CustomerId",
                    connection);
                command.Parameters.AddWithValue("@FirstName", cust.FirstName);
                command.Parameters.AddWithValue("@LastName", cust.LastName);
                command.Parameters.AddWithValue("@Phone", cust.Phone);
                command.Parameters.AddWithValue("@CustomerId", cust.CustomerId);

                connection.Open();
                numRowsAffected = (int)command.ExecuteNonQuery();
            }
            return numRowsAffected;
        }

        /// <summary>
        /// Gets a customer record based on standard parameterized SQL
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public Customer GetCustomerById(int CustomerId)
        {
            Customer customer = null;
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(
                    "SELECT cust_id, first_nm, last_nm, phone FROM [dbo].[tbl_customers] WHERE cust_id = @CustomerID",
                    connection);
                command.Parameters.AddWithValue("@CustomerId", CustomerId);
                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int r = 0; r < dataTable.Rows.Count; r++)
                {
                    customer = new Customer()
                    {
                        CustomerId = (int)dataTable.Rows[r]["cust_id"],
                        FirstName = (string)dataTable.Rows[r]["first_nm"],
                        LastName = (string)dataTable.Rows[r]["last_nm"],
                        Phone = (string)dataTable.Rows[r]["phone"]
                    };
                    break;
                }
            }
            return customer;
        }

        /// <summary>
        /// This access patten uses SELECT *, and binds to the expected columns while iterating
        /// through the DataTable rows.
        /// It also uses a WHERE clause constructed in a helper method.
        /// </summary>
        /// <param name="FirstNameSearch"></param>
        /// <param name="LastNameSearch"></param>
        /// <param name="PhoneSearch"></param>
        /// <returns></returns>
        public List<Customer> GetCustomerRecords(
            string FirstNameSearch,
            string LastNameSearch,
            string PhoneSearch)
        {
            var customers = new List<Customer>();

            // Create a search filter.
            var filter = new CustomersFilter()
            {
                FirstNameSearch = FirstNameSearch,
                LastNameSearch = LastNameSearch,
                PhoneSearch = PhoneSearch
            };

            using (var connection = new SqlConnection(connectionString))
            {
                // filter.WhereClause has hard-coded columns fromt he database.
                var adapter = new SqlDataAdapter($"SELECT * FROM [dbo].[tbl_customers] {filter.WhereClause}", connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int r = 0; r < dataTable.Rows.Count; r++)
                {
                    var c = new Customer()
                    {
                        CustomerId = (int)dataTable.Rows[r]["cust_id"],
                        FirstName = (string)dataTable.Rows[r]["first_nm"],
                        LastName = (string)dataTable.Rows[r]["last_nm"],
                        Phone = (string)dataTable.Rows[r]["phone"]
                    };
                    customers.Add(c);
                }
                return customers;
            }
        }

        /// <summary>
        /// This access pattern uses input and output parameters in a stored procedure, both of which
        /// may get changed by DMS.
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public int DeleteCustomerById(int CustomerId)
        {
            int rowsDeleted = 0;
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("proc_DeleteCustomerById",
                    connection)
                { CommandType = CommandType.StoredProcedure };

                // These map to parameters in the stored procedure, which are
                // subject to change according to migration rules.
                command.Parameters.AddWithValue("@Cust_Id", CustomerId);
                command.Parameters.Add("@Deleted_Count", SqlDbType.Int).Direction = ParameterDirection.Output;

                connection.Open();
                command.ExecuteNonQuery();
                rowsDeleted = (int)command.Parameters["@Deleted_Count"].Value;
            }
            return rowsDeleted;
        }
    }
}
