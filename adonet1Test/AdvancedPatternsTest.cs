using adonet1;
using adonet1.models;
using Microsoft.Extensions.Configuration;

namespace adonet1Test
{
    public class AdvancedPatternsTest
    {
        [Fact]
        public void Test_Dynamic_Table_Query()
        {
            var dataAccess = new AdvancedDataAccess();
            
            // This should challenge the detection tool with dynamic table/column names
            var result = dataAccess.GetDataFromDynamicTable("tbl_customers", "cust_id");
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Fragmented_Query_Builder()
        {
            var dataAccess = new AdvancedDataAccess();
            
            // Tests SQL built across multiple string operations
            var customers = dataAccess.GetCustomersWithComplexQuery("Smith");
            Assert.NotNull(customers);
        }

        [Fact]
        public void Test_Configurable_Data_Access()
        {
            var dataAccess = new ConfigurableDataAccess();
            
            // Tests SQL from configuration
            var result = dataAccess.ExecuteConfiguredQuery("AllCustomers");
            Assert.NotNull(result);
            
            // Tests fragmented query building
            var report = dataAccess.GetCustomerReport(true, true);
            Assert.NotNull(report);
        }

        [Fact]
        public void Test_Attribute_Based_SQL()
        {
            var config = ConfigHelper.GetConfiguration();
            var connectionString = config.GetSection("ConnectionStrings:DefaultConnection").Value;
            var dataAccess = new AttributeBasedDataAccess(connectionString);
            
            // Tests SQL stored in method attributes
            var result = dataAccess.GetCustomerByIdFromAttribute(1);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Cross_Project_Reporting()
        {
            var config = ConfigHelper.GetConfiguration();
            var connectionString = config.GetSection("ConnectionStrings:DefaultConnection").Value;
            Assert.NotNull(connectionString);
            var reporting = new adonet1.Reporting.CrossProjectReporting(connectionString);
            
            // Tests cross-project database object references
            var summary = reporting.GetCustomerSummary();
            Assert.NotNull(summary);
        }
    }
}
