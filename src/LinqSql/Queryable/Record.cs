﻿using System.Collections.Generic;

namespace System.Linq.Sql
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

        // ----- Operators ----- //

        public static Record operator |(Record a, Record b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            return new Record(a
                .Concat(b)
                .ToDictionary(x => x.Key, x => x.Value));
        }
    }
}
