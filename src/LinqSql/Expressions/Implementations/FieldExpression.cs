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
        /// <param name="expression">The fields <see cref="ASourceExpression"/>.</param>
        /// <param name="table">The table or table alias the field is exposed from.</param>
        /// <param name="field">The name of the field on the source.</param>
        /// <param name="source">The optional field expression this instance is mapping.</param>
        public FieldExpression(ASourceExpression expression, string table, string field, FieldExpression source = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be whitespace.", nameof(table));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Cannot be whitespace.", nameof(field));

            Expression = expression;
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

        /// <summary>Gets the fields <see cref="ASourceExpression"/>.</summary>
        public ASourceExpression Expression { get; } = null;

        /// <summary>Gets the optional source field this field is mapping.</summary>
        public FieldExpression Source { get; } = null;

        /// <summary>Gets the name of the table on the source.</summary>
        public string TableName { get; } = null;

        /// <summary>Gets the name of the field on the source.</summary>
        public string FieldName { get; } = null;
    }
}
