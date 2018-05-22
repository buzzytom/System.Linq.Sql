using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="AFieldExpression"/> provides the base class from which concrete field expressions are derived. This is an abstract class. This class extends <see cref="AExpression"/>.
    /// </summary>
    public abstract class AFieldExpression : AExpression
    {
        /// <summary>
        /// Initializes a new empty instance of <see cref="AFieldExpression"/> with the specified values.
        /// </summary>
        protected AFieldExpression(ASourceExpression expression, string table, string field)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be whitespace.", nameof(table));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Cannot be whitespace.", nameof(field));

            Expression = expression;
            TableName = table;
            FieldName = field;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        /// <remarks>This accept will render the declaration of the field, instead of its standard usage.</remarks>
        public abstract Expression AcceptDeclarationSql(ISqlExpressionVisitor visitor);

        // ----- Properties ----- //

        /// <summary>Gets the fields <see cref="ASourceExpression"/>.</summary>
        public ASourceExpression Expression { get; } = null;

        /// <summary>Gets the name of the table on the source.</summary>
        public string TableName { get; } = null;

        /// <summary>Gets the name of the field on the source.</summary>
        public string FieldName { get; } = null;
    }
}
