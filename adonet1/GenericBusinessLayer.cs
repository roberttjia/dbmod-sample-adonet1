using adonet1.models;
using adonet1;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adonet1
{
    /// <summary>
    /// Some business layer database class that uses GenericDataAccess
    /// </summary>
    public class GenericBusinessLayer
    {
        GenericDataAccess _genericDataAccess;
        public GenericBusinessLayer() 
        {
            _genericDataAccess = new GenericDataAccess(ConfigHelper.GetConfiguration().GetConnectionString("DefaultConnection"));
        }

        public List<Customer> GetCustomersByPartialLastName(string partialLastName)
        {
            string sql = "SELECT * FROM tbl_customers WHERE last_nm LIKE @PartialLastName";
            var genericParameters = new GenericSQLParams();
            genericParameters.Add("@PartialLastName", partialLastName);
            var dataSet = _genericDataAccess.ExecuteSelectSQL(sql, genericParameters);
            return ReadCustomersDS(dataSet);
        }

        public List<Customer> GetCustomersByPartialLastNameWithStoredProc(string partialLastName, int maxRows)
        {
            var genericParameters = new GenericSQLParams();
            genericParameters.Add("@LastNameSearch", partialLastName);
            genericParameters.Add("@MaxRows", maxRows);
            var dataSet = _genericDataAccess.ExecuteSelectSP("proc_SearchCustomersByLastName", genericParameters);

            return ReadCustomersDS(dataSet);
        }

        public Product InsertProductRecord(Product product)
        {
            string sql = "INSERT INTO tbl_products (name, code, description, price) VALUES (@Name, @Code, @Description, @Price)";
            var genericParameters = new GenericSQLParams();
            genericParameters.Add("@Name", product.Name);
            genericParameters.Add("@Code", product.Code);
            genericParameters.Add("@Description", product.Description);
            genericParameters.Add("@Price", product.Price);

            var rowsAffected = _genericDataAccess.ExecuteNonQuerySQL(sql, genericParameters);
            if (rowsAffected == 1)
            {
                string sql2 = "SELECT p.product_id from tbl_products p WHERE p.code = @Code";
                var genericParameters2 = new GenericSQLParams();
                genericParameters2.Add("@Code", product.Code);
                var ds = _genericDataAccess.ExecuteSelectSQL(sql2, genericParameters2);
                product.ProductId = (int)ds.Tables[0].Rows[0]["product_id"];
            }

            return product;
        }

        public Order InsertOrderRecord(Customer cust, Product prod, int quantity)
        {
            var order = new Order()
            {
                CustomerId = cust.CustomerId,
                ProductId = prod.ProductId,
                Quantity = quantity,
                TotalPrice = quantity * prod.Price
            };
            string sql = "INSERT INTO tbl_orders (cust_id, product_id, quantity, total_price) VALUES (@CustId, @ProductId, @Quantity, @TotalPrice)";
            var genericParameters = new GenericSQLParams();
            genericParameters.Add("@CustId", cust.CustomerId);
            genericParameters.Add("@ProductId", prod.ProductId);
            genericParameters.Add("@Quantity", quantity);
            genericParameters.Add("@TotalPrice", order.TotalPrice);

            var rowsAffected = _genericDataAccess.ExecuteNonQuerySQL(sql, genericParameters);

            // Get the latest Order Id. This is just for testing as this is susceptible to race conditions.
            string sql2 = "SELECT MAX(order_id) FROM tbl_orders";
            var ds =_genericDataAccess.ExecuteSelectSQL(sql2, new GenericSQLParams());
            order.OrderId = (int)ds.Tables[0].Rows[0][0];

            return order;
        }

        private List<Customer> ReadCustomersDS(DataSet dataSet)
        {
            var customers = new List<Customer>();

            var custTable = dataSet.Tables[0];
            for (int r = 0; r < custTable.Rows.Count; r++)
            {
                var c = new Customer()
                {
                    CustomerId = (int)custTable.Rows[r]["cust_id"],
                    FirstName = (string)custTable.Rows[r]["first_nm"],
                    LastName = (string)custTable.Rows[r]["last_nm"],
                    Phone = (string)custTable.Rows[r]["phone"]
                };
                customers.Add(c);
            }
            return customers;
        }
    }
}
