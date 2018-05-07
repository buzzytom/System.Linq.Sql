using System.Collections.Generic;

namespace System.Linq.Sql.Queryable
{
    /// <summary>
    /// <see cref="Record"/> is the map of a single result row of a query. The mapping correlates a table name or alias to a single <see cref="RecordItem"/>.
    /// </summary>
    public class Record : Dictionary<string, RecordItem>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Record"/>, with the specified map.
        /// </summary>
        /// <param name="record">The mapping which the record represents.</param>
        public Record(Dictionary<string, RecordItem> record)
            : base(record)
        { }
    }
}
