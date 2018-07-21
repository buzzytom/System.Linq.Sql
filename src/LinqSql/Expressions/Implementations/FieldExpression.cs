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
        /// <param name="valueExpression">The fields <see cref="AExpression"/> representing the value of the field.</param>
        /// <param name="table">The table or table alias the field is exposed from.</param>
        /// <param name="field">The name of the field on the source.</param>
        /// <param name="sourceExpression">The optional field expression this instance is mapping.</param>
        public FieldExpression(AExpression valueExpression, string table, string field, FieldExpression sourceExpression = null)
        {
            if (valueExpression == null)
                throw new ArgumentNullException(nameof(valueExpression));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be whitespace.", nameof(table));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Cannot be whitespace.", nameof(field));

            ValueExpression = valueExpression;
            TableName = table;
            FieldName = field;
            SourceExpression = sourceExpression;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(IQueryExpressionVisitor visitor)
        {
            return visitor.VisitField(this);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        public virtual Expression AcceptDeclarationSql(IQueryExpressionVisitor visitor)
        {
            return visitor.VisitFieldDeclaration(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets the fields <see cref="AExpression"/> which provides its value.</summary>
        public AExpression ValueExpression { get; } = null;

        /// <summary>Gets the name of the table on the source.</summary>
        public string TableName { get; } = null;

        /// <summary>Gets the name of the field on the source.</summary>
        public string FieldName { get; } = null;

        /// <summary>Gets the optional source field this field is mapping.</summary>
        public FieldExpression SourceExpression { get; } = null;
    }
}
