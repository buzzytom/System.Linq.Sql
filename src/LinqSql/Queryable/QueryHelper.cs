using System.Collections.Generic;

namespace System.Linq.Sql
{
    public static class QueryHelper
    {
        public static IEnumerable<RecordItem> SelectRecordItems(this IEnumerable<Record> records)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));

            return records.Select(x => x.Values.FirstOrDefault());
        }

        public static IEnumerable<RecordItem> SelectRecordItems(this IEnumerable<Record> records, string table)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be whitespace.", nameof(table));

            return records.Select(x => x[table]);
        }

        public static IEnumerable<ILookup<string, object>> Flatten(this IEnumerable<Record> records)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));

            return records.Select((Record record) =>
            {
                return record
                    .SelectMany(x => x.Value)
                    .ToLookup(x => x.Key, x => x.Value);
            });
        }
    }
}
