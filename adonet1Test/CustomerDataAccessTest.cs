using adonet1.models;
using adonet1;

namespace adonet1Test
{
    public class CustomerDataAccessTest
    {
        [Fact]
        public void Test_Insert_And_Retrieve_Customer()
        {
            var dataAccess = new CustomerDataAccess();

            // Insert a customer with a unique last name
            Guid guid = Guid.NewGuid();
            string part = guid.ToString("N").Substring(0, 8);
            var cust = new Customer()
            {
                FirstName = "John",
                LastName = $"Doe{part}",
                Phone = "212-333-4455"
            };
            cust = dataAccess.InsertCustomerRecord(cust);
            Assert.True(cust.CustomerId > 0);

            // Query customer just inserted.
            var customers = dataAccess.GetCustomerRecords("", cust.LastName, "");
            var cust2 = customers[0];
            Assert.Single(customers); // This is true most of the time.
            Assert.Equal(cust.CustomerId, cust2.CustomerId);
        }

        [Fact]
        public void Test_Insert_And_Get_Customer_By_Id()
        {
            var dataAccess = new CustomerDataAccess();

            // Insert a customer with a unique last name
            Guid guid = Guid.NewGuid();
            string part = guid.ToString("N").Substring(0, 8);
            var cust = new Customer()
            {
                FirstName = "Jane",
                LastName = $"Smith{part}",
                Phone = "212-555-1234"
            };
            cust = dataAccess.InsertCustomerRecord(cust);
            Assert.True(cust.CustomerId > 0);

            // Get customer just inserted.
            var cust2 = dataAccess.GetCustomerById(cust.CustomerId);
            Assert.Equal(cust.CustomerId, cust2.CustomerId);
            Assert.Equal(cust.FirstName, cust2.FirstName);
            Assert.Equal(cust.LastName, cust2.LastName);
            Assert.Equal(cust.Phone, cust2.Phone);
        }

        [Fact]
        public void Test_Insert_And_Update_Customer_By_Id()
        {
            var dataAccess = new CustomerDataAccess();

            // Insert a customer with a unique last name
            Guid guid = Guid.NewGuid();
            string part = guid.ToString("N").Substring(0, 8);
            var cust = new Customer()
            {
                FirstName = "Jane",
                LastName = $"Smith{part}",
                Phone = "212-555-1234"
            };
            cust = dataAccess.InsertCustomerRecord(cust);
            Assert.True(cust.CustomerId > 0);


            // Update customer just inserted.
            cust.FirstName = $"Alias{part}";
            dataAccess.UpdateCustomerRecord(cust);

            // Retrieve and validate the update happened
            var cust2 = dataAccess.GetCustomerById(cust.CustomerId);
            Assert.Equal(cust.CustomerId, cust2.CustomerId);
            Assert.Equal(cust.FirstName, cust2.FirstName);
            Assert.Equal(cust.LastName, cust2.LastName);
            Assert.Equal(cust.Phone, cust2.Phone);
        }

        [Fact]
        public void Test_SP_With_In_Out_Params()
        {
            var dataAccess = new CustomerDataAccess();

            // Insert a customer with a unique last name
            Guid guid = Guid.NewGuid();
            string part = guid.ToString("N").Substring(0, 8);
            var cust = new Customer()
            {
                FirstName = "John",
                LastName = $"Doe{part}",
                Phone = "212-333-4455"
            };
            cust = dataAccess.InsertCustomerRecord(cust);
            Assert.True(cust.CustomerId > 0);

            int rowsDeleted = dataAccess.DeleteCustomerById(cust.CustomerId);
            Assert.Equal<int>(1, rowsDeleted);
        }
    }
}