using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="SqliteExpressionVisitor"/> is a custom implementation of <see cref="SqlExpressionVisitor"/>, designed for queries on an SQLite database.
    /// </summary>
    public class SqliteExpressionVisitor : SqlExpressionVisitor
    {
        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public override Expression VisitBoolean(BooleanExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (expression.Value)
                Builder.Append("1");
            else
                Builder.Append("0");

            return expression;
        }
    }
}
