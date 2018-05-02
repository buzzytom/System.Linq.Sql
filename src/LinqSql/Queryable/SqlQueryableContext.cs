using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LinqSql.Queryable
{
    using Expressions;

    internal static class SqlQueryableContext
    {
        internal static IEnumerable<Record> ExecuteQuery(this DbConnection connection, ASourceExpression expression, SqlExpressionVisitor visitor)
        {
            // Create the outer select expression
            SelectExpression select = expression as SelectExpression;
            if (select == null)
                select = new SelectExpression(expression, expression.Fields.Select(x => x.Alias));

            // Ensure the connection is open
            if (connection.State != ConnectionState.Open)
                throw new InvalidOperationException("The connection must be open to execute a query.");

            // Create the command
            LinkedList<Record> items = new LinkedList<Record>();
            using (DbCommand command = connection.CreateCommand())
            {
                // Prepare the query
                Query query = visitor.GenerateQuery(select);
                command.CommandText = query.Sql;
                foreach (KeyValuePair<string, object> pair in query.Parameters)
                    command.AddParameter(pair.Key, pair.Value);

                // Execute the query
                using (DbDataReader reader = command.ExecuteReader())
                {
                    ILookup<string, CommandField> fields = reader.GetFieldMap(select.Fields);
                    while (reader.Read())
                    {
                        // Read the row of the result set
                        Dictionary<string, RecordItem> row = fields
                            .Select(x => ReadItem(reader, x, x.Key))
                            .ToDictionary(x => x.Key);

                        // At this point the row has been created
                        items.AddLast(new Record(row));
                    }
                }
            }
            return items;
        }

        private static RecordItem ReadItem(this DbDataReader reader, IEnumerable<CommandField> fields, string type)
        {
            RecordItem item = new RecordItem(type);
            foreach (CommandField field in fields)
            {
                object value = reader[field.Ordinal];
                if (value is DBNull)
                    value = null;
                item[field.FieldName] = value;
            }
            return item;
        }

        private static ILookup<string, CommandField> GetFieldMap(this DbDataReader reader, IEnumerable<FieldExpression> fields)
        {
            // TODO - Resolve the table alias to give the field
            return fields
                .Select(x => new CommandField("table", x.Alias, reader.GetOrdinal(x.Alias)))
                .ToLookup(x => x.Table);
        }
    }
}
