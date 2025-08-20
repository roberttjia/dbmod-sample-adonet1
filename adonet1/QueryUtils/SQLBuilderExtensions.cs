using System.Text;

namespace adonet1.QueryUtils
{
    // Extension methods for fluent SQL building - more fragmentation
    public static class SQLBuilderExtensions
    {
        public static StringBuilder AppendSelect(this StringBuilder sb)
        {
            return sb.Append($"SELECT {SQLFragments.GetCustomerSelectList()}");
        }

        public static StringBuilder AppendFrom(this StringBuilder sb)
        {
            return sb.Append($" FROM [dbo].{SQLFragments.CUSTOMERS_TABLE} c");
        }

        public static StringBuilder AppendWhere(this StringBuilder sb)
        {
            return sb.Append($" WHERE c.{SQLFragments.CUST_ID_COL} > 0");
        }
    }
}
