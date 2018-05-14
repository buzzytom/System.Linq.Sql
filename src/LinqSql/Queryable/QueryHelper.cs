using System.Collections.Generic;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="QueryHelper"/> defines utility methods for working with <see cref="SqlQueryable"/> related constructs.
    /// </summary>
    public static class QueryHelper
    {
        /// <summary>
        /// Selects the single <see cref="RecordItem"/> out of each <see cref="Record"/> in the specified collection. If there is not exactly one item in each record, an exception is thrown.
        /// </summary>
        /// <param name="records">The collection to select the record items from.</param>
        /// <returns>A collection of the single <see cref="RecordItem"/> from each of the specified records.</returns>
        public static IEnumerable<RecordItem> SelectRecordItems(this IEnumerable<Record> records)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));

            return records.Select(x => x.Values.Single());
        }

        /// <summary>
        /// Selects the specified <see cref="RecordItem"/> out of each <see cref="Record"/> in the specified collection using the specified table key. If a <see cref="Record"/> does not contain an item for the specified table, a <see cref="KeyNotFoundException"/> is thrown.
        /// </summary>
        /// <param name="records">The collection to select the record items from.</param>
        /// <param name="table">The table to select from the records.</param>
        /// <returns>A collection of the specified record table items.</returns>
        public static IEnumerable<RecordItem> SelectRecordItems(this IEnumerable<Record> records, string table)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be whitespace.", nameof(table));

            return records.Select(x => x[table]);
        }

        /// <summary>
        /// Merges the each record item of the specified <see cref="Record"/> into a collection of dictionaries. An <see cref="ArgumentException"/> is thrown if a result record has duplicate keys.
        /// </summary>
        /// <param name="records">The records to flatten.</param>
        /// <returns>A collection of each flattened record.</returns>
        public static IEnumerable<Dictionary<string, object>> Flatten(this IEnumerable<Record> records)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));

            return records.Select(record =>
            {
                return record
                    .SelectMany(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.Value);
            });
        }
    }
}
