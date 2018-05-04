using System.Linq.Expressions;

namespace System.Linq.Sql.Expressions
{
    /// <summary>
    /// Represents a column selection of a database table.
    /// </summary>
    public class FieldExpression : AExpression
    {
        private readonly string table = null;
        private readonly string field = null;

        /// <summary>
        /// Initializes a new instance of <see cref="FieldExpression"/>, selecting the specified field from the specified source.
        /// </summary>
        /// <param name="source">The source which the field is selecting its data from.</param>
        /// <param name="table">The table or table alias the field is exposed from.</param>
        /// <param name="field">The name of the field on the source.</param>
        public FieldExpression(string table, string field)
        {
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be whitespace.", nameof(table));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Cannot be whitespace.", nameof(field));

            this.table = table;
            this.field = field;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is ISqlExpressionVisitor sqlVisitor)
                return sqlVisitor.VisitField(this);
            else
                return base.Accept(visitor);
        }

        // ----- Properties ----- //

        /// <summary>Gets the name of the table on the source.</summary>
        public string TableName => table;

        /// <summary>Gets the name of the field on the source.</summary>
        public string FieldName => field;
    }
}
