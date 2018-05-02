using System;
using System.Linq.Expressions;

namespace LinqSql.Expressions
{
    /// <summary>
    /// Represents a column selection of a database table.
    /// </summary>
    public class FieldExpression : Expression
    {
        private readonly string field = null;
        private readonly string alias = null;
        private readonly ASourceExpression source = null;

        /// <summary>
        /// Initializes a new instance of <see cref="FieldExpression"/>, selecting the specified field from the specified source.
        /// </summary>
        /// <param name="source">The source which the field is selecting its data from.</param>
        /// <param name="field">The name of the field on the source.</param>
        /// <param name="alias">The alias the field will be exposed as.</param>
        public FieldExpression(ASourceExpression source, string field, string alias)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Cannot be whitespace.", nameof(field));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentException("Cannot be whitespace.", nameof(alias));

            this.source = source;
            this.field = field;
            this.alias = alias;
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

        /// <summary>Gets the name of the field on the <see cref="Source"/>.</summary>
        public string FieldName => field;

        /// <summary>Gets the alias the field is exposed as.</summary>
        public string Alias => alias;

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public ASourceExpression Source => source;
    }
}
