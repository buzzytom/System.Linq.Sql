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

        /// <summary>
        /// Gets the first value of the first table of the first row and converts it to the specified generic type.
        /// </summary>
        /// <typeparam name="TResult">The type to convert the result to.</typeparam>
        /// <param name="records">The source sequence to get the value from.</param>
        /// <returns>The value converted to the specified type.</returns>
        /// <remarks>Additional rows, tables and columns are ignored.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if there is not at least one value available to get.</exception>
        /// <exception cref="Exception">Thrown if the result scalar value could not be converted to the generic type.</exception>
        public static TResult GetScalar<TResult>(this IEnumerable<Record> records)
        {
            // Get the single row
            Record row = records.FirstOrDefault();
            if (row == null)
                throw new InvalidOperationException($"To get a scalar value the {nameof(Record)} sequence must contain at least one row.");

            // Get the single table
            RecordItem item = row.Values.FirstOrDefault();
            if (item == null)
                throw new InvalidOperationException($"To get a scalar value the {nameof(Record)} sequence must contain at least one table on its first row.");

            // Get the value from the item
            object result = item.Values.FirstOrDefault();
            if (result == null)
                throw new InvalidOperationException($"To get a scalar value the {nameof(Record)} sequence must contain at least one column on the first table on its first row.");

            // Check for null
            if (result == null)
            {
                if (typeof(TResult).IsValueType)
                    throw new InvalidOperationException($"The scalar value of the sequence is null, this is not allowed with the value type '{typeof(TResult).Name}'.");
                return default(TResult);
            }

            // Attempt cast
            if (result is TResult)
                return (TResult)result;

            // Attempt convert
            try
            {
                return (TResult)Convert.ChangeType(result, typeof(TResult));
            }
            catch
            {
                throw new Exception($"{nameof(GetScalar)} failed to convert the result of type '{result.GetType().Name}' to '{typeof(TResult).Name}'.");
            }
        }
    }
}
