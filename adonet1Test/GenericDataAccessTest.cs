using adonet1;
using adonet1.models;

namespace adonet1Test
{
    public class GenericDataAccessTest
    {
        [Fact]
        public void Test_ExecuteSelectSQL()
        {
            string prefix = "UniqueLastNameABC123";
            var dataAccess = new CustomerDataAccess();

            // Insert a customers with a unique last names
            for (int i = 0; i < 10; i++)
            {
                Guid guid = Guid.NewGuid();
                string part = guid.ToString("N").Substring(0, 8);
                var cust = new Customer()
                {
                    FirstName = "Joe",
                    LastName = $"{prefix}{part}",
                    Phone = "800-543-3245"
                };
                cust = dataAccess.InsertCustomerRecord(cust);
                Assert.True(cust.CustomerId > 0);
            }

            // This is the test
            var bl = new GenericBusinessLayer();
            var customers = bl.GetCustomersByPartialLastName($"{prefix}%");
            Assert.True(customers.Count >= 10);

            // Clean up
            foreach (var cust in customers)
            {
                var rowCount = dataAccess.DeleteCustomerById(cust.CustomerId);
                Assert.True(rowCount == 1);
            }

        }
        [Fact]
        public void Test_ExecuteSelectSP()
        {
            string prefix = "UniqueLastNameDEF456";
            var dataAccess = new CustomerDataAccess();

            // Insert 10 customers with a unique last names
            for (int i = 0; i < 10; i++)
            {
                Guid guid = Guid.NewGuid();
                string part = guid.ToString("N").Substring(0, 8);
                var cust = new Customer()
                {
                    FirstName = "Joe",
                    LastName = $"{prefix}{part}",
                    Phone = "800-543-3245"
                };
                cust = dataAccess.InsertCustomerRecord(cust);
                Assert.True(cust.CustomerId > 0);
            }

            // This is the test
            var bl = new GenericBusinessLayer();
            var customers = bl.GetCustomersByPartialLastNameWithStoredProc($"{prefix}%", 5);
            Assert.True(customers.Count == 5);

            // Clean up - pass in int.MaxValue which SQL Server int should be able to handle
            var customersDelete = bl.GetCustomersByPartialLastNameWithStoredProc($"{prefix}%", int.MaxValue);
            foreach (var cust in customersDelete)
            {
                var rowCount = dataAccess.DeleteCustomerById(cust.CustomerId);
                Assert.True(rowCount == 1);
            }
        }
    }
}
