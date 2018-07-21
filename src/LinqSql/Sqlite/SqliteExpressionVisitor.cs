﻿using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="SqliteExpressionVisitor"/> is a custom implementation of <see cref="QueryExpressionVisitor"/>, designed for queries on an SQLite database.
    /// </summary>
    public class SqliteExpressionVisitor : QueryExpressionVisitor
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

        /// <summary>
        /// Renders the specified limit options.
        /// </summary>
        /// <param name="skip">The number of result rows to skip before reading.</param>
        /// <param name="take"></param>
        protected override void RenderLimit(long skip, long take)
        {
            Builder.Append(" limit ");
            Builder.Append(take);
            Builder.Append(" offset ");
            Builder.Append(skip);
        }
    }
}
