using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Sql
{
    public partial class LinqTranslatorVisitor
    {
        private FieldExpression VisitField(MethodCallExpression expression)
        {
            if (sources == null)
                throw new InvalidOperationException($"A field can only be visited if an {nameof(ASourceExpression)} has previously been visited.");

            // Resolve the field name
            ConstantExpression fieldNameExpression = expression.Arguments.FirstOrDefault() as ConstantExpression;
            if (!expression.Method.DeclaringType.IsAssignableFrom(typeof(Dictionary<string, object>)))
                throw new InvalidOperationException("The declaring type for a field expression must be a Dictionary<string, object>.");
            if (expression.Method.ReturnType != typeof(object))
                throw new InvalidOperationException("The return type for a field expression must be type of object.");
            if (expression.Arguments.Count != 1 || fieldNameExpression?.Type != typeof(string))
                throw new InvalidOperationException("The field name indexer for the field expression must contain exactly one string parameter.");

            // Resolve the table name
            MethodCallExpression source = expression.Object as MethodCallExpression;
            ConstantExpression tableNameExpression = source?.Arguments.FirstOrDefault() as ConstantExpression;
            if (source == null)
                throw new InvalidOperationException($"The table instance object could not be resolved for the field: {fieldNameExpression.Value.ToString()}");
            if (source.Method.Name != "get_Item")
                throw new NotSupportedException("Only an array indexer can be used to resolve a fields table name.");
            if (!source.Method.ReturnType.IsAssignableFrom(typeof(RecordItem)))
                throw new InvalidOperationException($"When mapping a field, the table name must map to a {nameof(RecordItem)}.");
            if (source.Arguments.Count != 1 || tableNameExpression?.Type != typeof(string))
                throw new InvalidOperationException("The table name indexer for the field expression must contain exactly one string parameter.");

            // Resolve the indexer values
            string tableName = tableNameExpression.Value as string;
            string fieldName = fieldNameExpression.Value as string;
            if (string.IsNullOrWhiteSpace(tableName))
                throw new InvalidOperationException("The table name cannot be empty.");
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new InvalidOperationException("The field name cannot be empty.");

            // Get the field from the current source
            FieldExpression found = sources?
                .SelectMany(x => x.Fields)
                .FirstOrDefault(x => x.TableName == tableName && x.FieldName == fieldName);
            if (found == null)
                throw new KeyNotFoundException($"The field [{tableName}].[{fieldName}] could not be found on the current source expression.");

            return found;
        }
    }
}
