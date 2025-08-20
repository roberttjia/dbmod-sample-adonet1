using adonet1.models;
using adonet1.QueryUtils;
using System;

namespace adonet1
{
    class Program
    {
        /// <summary>
        /// Skeleton main - does nothing. Code to exercise Data Access layer in test project
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var dataAccess = new CustomerDataAccess();

            // Insert example
            Guid guid = Guid.NewGuid();
            string part = guid.ToString("N").Substring(0, 4);
            var cust = new Customer()
            {
                FirstName = "John",
                LastName = $"Doe{part}",
                Phone = "212-333-4455"
            };
            cust = dataAccess.InsertCustomerRecord(cust);

            // Query example
            var customers = dataAccess.GetCustomerRecords("", "D", "");
            Console.WriteLine($"Retrieved {customers.Count} records");
        }
    }
}
