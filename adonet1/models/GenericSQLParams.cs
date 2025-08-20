using System.Collections.Generic;

namespace adonet1.models
{
    public class GenericSQLParams : Dictionary<string, object>
    {
        public void Add(string name, object value) => this[name] = value;
    }
}
