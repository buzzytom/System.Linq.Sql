﻿using System.Linq.Expressions;
using System.Linq.Sql.Expressions;
using System.Reflection;

namespace System.Linq.Sql.Queryable
{
    /// <summary>
    /// <see cref="SqlTranslatorVisitor"/> translates an expression call to an sql expression tree.
    /// </summary>
    public class SqlTranslatorVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SqlTranslatorVisitor"/>.
        /// </summary>
        public SqlTranslatorVisitor()
        { }

        /// <summary>
        /// Translates the specified expression and its subtree to an <see cref="AExpression"/>.
        /// </summary>
        /// <param name="expression">The expression to modify.</param>
        /// <returns>The converted expression tree.</returns>
        public static Expression Translate(Expression expression)
        {
            SqlTranslatorVisitor visitor = new SqlTranslatorVisitor();
            return visitor.Visit(expression);
        }

        /// <summary>
        /// Dispatches the expression to one of the more specialized visit methods in this class converting the expression and its children to an <see cref="AExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The expression modified to an <see cref="AExpression"/>, otherwise a thrown exception.</returns>
        public override Expression Visit(Expression node)
        {
            if (node is AExpression)
                return node;
            else
                return base.Visit(node);
        }

        private T Visit<T>(Expression expression)
            where T : AExpression
        {
            T source = Visit(StripQuotes(expression)) as T;
            if (source == null)
                throw new NotSupportedException($"Could not convert the expression to an {nameof(T)}.");
            return source;
        }

        /// <summary>
        /// Visits the children of the System.Linq.Expressions.MethodCallExpression, translating the expression to an <see cref="ASourceExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The specified expression converted to an <see cref="ASourceExpression"/>; otherwise a thrown exception.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Get the associated method
            MethodInfo method = node.Method;
            if (method.DeclaringType != typeof(Linq.Queryable))
                throw new InvalidOperationException("Cannot translate the method '{method.Name}' unless the source is a System.Linq.Queryable instance.");

            // Decode specific method node handling
            if (method.Name == "Where")
            {
                ASourceExpression source = Visit<ASourceExpression>(node.Arguments[0]);
                APredicateExpression predicate = Visit<APredicateExpression>(node.Arguments[1]);
                return new WhereExpression(source, predicate);
            }

            throw new NotSupportedException($"Cannot translate the method '{method.Name}' because it's not known by the sql translator.");
        }

        /// <summary>
        /// Visits the System.Linq.Expressions.ConstantExpression and converts it into either a <see cref="NullExpression"/> if the value is null otherwise a <see cref="LiteralExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The specified expression converted to either a <see cref="NullExpression"/> or <see cref="LiteralExpression"/>.</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
                return new NullExpression();
            else
                return new LiteralExpression(node.Value);
        }

        /// <summary>
        /// Visits the expression represented by the lambda and converts it into an <see cref="APredicateExpression"/>.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression; an <see cref="APredicateExpression"/>.</returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            // TODO - Evaluate the subtree

            throw new NotImplementedException();
        }

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
                expression = ((UnaryExpression)expression).Operand;
            return expression;
        }
    }
}
