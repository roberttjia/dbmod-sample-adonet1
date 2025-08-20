using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adonet1.QueryUtils
{
    public class CustomersFilter
    {
        public string FirstNameSearch { get; set; } = "";
        public string LastNameSearch { get; set; } = "";
        public string PhoneSearch { get; set; } = "";

        public CustomersFilter()
        {
        }

        public string WhereClause
        {
            get
            {
                if (string.IsNullOrEmpty(FirstNameSearch) && string.IsNullOrEmpty(LastNameSearch) && string.IsNullOrEmpty(PhoneSearch))
                {
                    return "";
                }
                else
                {
                    var sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(FirstNameSearch))
                    {
                        sb.Append($"first_nm LIKE '%{FirstNameSearch}%'");
                    }
                    if (!string.IsNullOrEmpty(LastNameSearch))
                    {
                        if (sb.Length > 0)
                            sb.Append(" AND ");
                        sb.Append($"last_nm LIKE '%{LastNameSearch}%'");
                    }
                    if (!string.IsNullOrEmpty(PhoneSearch))
                    {
                        if (sb.Length > 0)
                            sb.Append(" AND ");
                        sb.Append($"phone LIKE '%{PhoneSearch}%'");
                    }
                    return "WHERE " + sb.ToString();
                }
            }
        }
    }
}
