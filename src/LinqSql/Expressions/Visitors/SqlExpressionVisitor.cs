using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LinqSql.Expressions
{
    /// <summary>
    /// <see cref="SqlExpressionVisitor"/> is an implementation of <see cref="ISqlExpressionVisitor"/>, which the visit implementations generate an SQL representation of .
    /// </summary>
    public class SqlExpressionVisitor : ExpressionVisitor, ISqlExpressionVisitor
    {
        private readonly StringBuilder builder = new StringBuilder();
        private readonly SqlVisitorContext context = new SqlVisitorContext();

        /// <summary>
        /// Creates a new instance of <see cref="SqlExpressionVisitor"/>.
        /// </summary>
        public SqlExpressionVisitor()
        {
        }

        /// <summary>
        /// Generates the sql representation of the specified expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree to generate sql for.</param>
        /// <returns>The sql representation of the specified expression.</returns>
        /// <remarks>This method will clear any state currently executing on the visitor.</remarks>
        public string GenerateSql(SelectExpression expression)
        {
            builder.Clear();
            context.Clear();
            builder.Append("select * from ");
            Visit(expression);
            return builder.ToString();
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitTable(TableExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder.Append($"{expression.Table} as [{expression.Alias}]");

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitSelect(SelectExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder.Append("(select ");
            VisitFields(expression.Fields);
            builder.Append(" from ");
            Visit(expression.Source);
            builder.Append($")as[{context.GetSource(expression)}]");

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitField(FieldExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            throw new NotImplementedException();

            //return expression;
        }

        /// <summary>
        /// Visits the specified expressions.
        /// </summary>
        /// <param name="fields">A collection of expressions to visit.</param>
        protected virtual void VisitFields(IEnumerable<FieldExpression> fields)
        {
            bool comma = false;
            foreach (FieldExpression field in fields)
            {
                if (comma)
                    builder.Append(",");
                builder.Append($"[{context.GetSource(field.Source)}].[{field.FieldName}]as[{field.Alias}]");
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
