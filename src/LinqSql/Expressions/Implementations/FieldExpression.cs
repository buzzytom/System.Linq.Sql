using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// Represents a column selection of a database table.
    /// </summary>
    public class FieldExpression : AExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FieldExpression"/>, selecting the specified field from the specified source.
        /// </summary>
        /// <param name="fields">The field expression map the field is part of.</param>
        /// <param name="source">The source expression which the field is present on.</param>
        /// <param name="table">The table or table alias the field is exposed from.</param>
        /// <param name="field">The name of the field on the source.</param>
#if DEBUG
        public
#else
        internal
#endif
        FieldExpression(FieldExpressions fields, ASourceExpression source, string table, string field)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be whitespace.", nameof(table));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Cannot be whitespace.", nameof(field));

            Fields = fields;
            Source = source;
            TableName = table;
            FieldName = field;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitField(this);
        }

        // ----- Properties ----- //

        /// <summary>The field expression map the field is part of.</summary>
        public FieldExpressions Fields { get; } = null;

        /// <summary>Gets the source expression this field expression exists on.</summary>
        public ASourceExpression Source { get; } = null;

        /// <summary>Gets the name of the table on the source.</summary>
        public string TableName { get; } = null;

        /// <summary>Gets the name of the field on the source.</summary>
        public string FieldName { get; } = null;
    }
}
