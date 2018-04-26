using System;
using System.Collections.Generic;
using System.Text;

namespace LinqSql.Expressions
{
    /// <summary>
    /// <see cref="SqlStandardExpressionVisitor"/> is an implementation of <see cref="AExpressionVisitor"/>, which the visit implementations generate an SQL representation of .
    /// </summary>
    public class SqlStandardExpressionVisitor : AExpressionVisitor
    {
        private StringBuilder builder = new StringBuilder();
        private SqlVisitorContext context = new SqlVisitorContext();

        /// <summary>
        /// Creates a new instance of <see cref="SqlStandardExpressionVisitor"/>.
        /// </summary>
        public SqlStandardExpressionVisitor()
        {
        }

        /// <summary>
        /// Generates the sql representation of the specified expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree to generate sql for.</param>
        /// <returns>The sql representation of the specified expression.</returns>
        /// <remarks>This method will clear any state currently executing on the visitor.</remarks>
        public string GenerateSql(AExpression expression)
        {
            builder.Clear();
            context.Clear();

            if (expression is ASourceExpression)
                builder.Append("select * from ");

            expression.Accept(this);
            return builder.ToString();
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public override void VisitTable(TableExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder.Append($"{expression.Table} as [{expression.Alias}]");
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public override void VisitSelect(SelectExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder.Append("(select ");
            VisitFields(expression.Fields);
            builder.Append(" from ");
            expression.Source.Accept(this);
            builder.Append($")as[{context.GetSource(expression)}]");
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public override void VisitField(FieldExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            throw new NotImplementedException();
        }

        private void VisitFields(IEnumerable<FieldExpression> fields)
        {
            bool comma = false;
            foreach (FieldExpression field in fields)
            {
                if (comma)
                    builder.Append(",");
                builder.Append($"[{context.GetSource(field.Source)}].[{field.Field}]as[{field.Alias}]");
                comma = true;
            }
        }

        // ----- Properties ----- //

        /// <summary>Gets the string builder used to form the query.</summary>
        protected StringBuilder Builder => builder;

        /// <summary>Gets the vistor context used during the visitation of expressions.</summary>
        protected SqlVisitorContext Context => context;

        /// <summary>Gets the current sql state of the visitor.</summary>
        public string SqlState => builder.ToString();
    }
}
