using System.Collections.Generic;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="RecordItem"/> is a map which represents a single selection from a result record of a query. The map correlates column names to their associated data objects.
    /// </summary>
    public class RecordItem : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RecordItem"/>, with the specified key and map.
        /// </summary>
        /// <param name="key">The table or selection key which the record item represents.</param>
        /// <param name="data">The mapping which the record represents.</param>
        public RecordItem(string key, Dictionary<string, object> data = null)
            : base(data ?? new Dictionary<string, object>())
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cannot be null or whitespace.", nameof(key));

            Key = key;
        }

        // ----- Properties ----- //

        /// <summary>Gets the table or selection key which this item represents.</summary>
        public string Key { private set; get; }
    }
}
