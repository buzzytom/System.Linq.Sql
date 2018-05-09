﻿using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Sql
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
                throw new NotSupportedException($"Could not convert the expression to an {typeof(T).Name}.");
            return source;
        }

        /// <summary>
        /// Visits the children of the System.Linq.Expressions.MethodCallExpression, translating the expression to an <see cref="ASourceExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The specified expression converted to an <see cref="ASourceExpression"/>; otherwise a thrown exception.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "get_Item":
                    return VisitField(node);
                case "Where":
                    return VisitWhere(node);
                default:
                    throw new NotSupportedException($"Cannot translate the method '{node.Method.Name}' because it's not known by the sql translator.");
            }
        }

        /// <summary>
        /// Visits the System.Linq.Expressions.ConstantExpression and converts it into either a <see cref="NullExpression"/> if the value is null, <see cref="BooleanExpression"/> if the expression is a boolean, otherwise a <see cref="LiteralExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The specified expression converted to either a <see cref="NullExpression"/>, <see cref="BooleanExpression"/> or <see cref="LiteralExpression"/>.</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
                return new NullExpression();
            else if (typeof(bool).IsAssignableFrom(node.Value.GetType()))
                return new BooleanExpression((bool)node.Value);
            else
                return new LiteralExpression(node.Value);
        }

        /// <summary>
        /// Visits the children of the System.Linq.Expressions.BinaryExpression. This implementation converts the expression into a <see cref="CompositeExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit</param>
        /// <returns>The specified binary expression converted to a <see cref="CompositeExpression"/>.</returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            AExpression left = Visit<AExpression>(node.Left);
            AExpression right = Visit<AExpression>(node.Right);
            return new CompositeExpression(left, right, GetCompositeOperator(node.NodeType));
        }

        private CompositeOperator GetCompositeOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return CompositeOperator.And;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return CompositeOperator.Or;
                case ExpressionType.GreaterThan:
                    return CompositeOperator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return CompositeOperator.GreaterThanOrEqual;
                case ExpressionType.LessThan:
                    return CompositeOperator.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return CompositeOperator.LessThanOrEqual;
                case ExpressionType.Equal:
                    return CompositeOperator.Equal;
                case ExpressionType.NotEqual:
                    return CompositeOperator.NotEqual;
                default:
                    throw new NotSupportedException($"Cannot convert {type.ToString()} to a {nameof(CompositeOperator)}.");
            }
        }

        private WhereExpression VisitWhere(MethodCallExpression expression)
        {
            if (expression.Method.DeclaringType != typeof(Queryable))
                throw new InvalidOperationException("The declaring type for a Where expression must be a Queryable.");

            ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);
            LambdaExpression lambda = (LambdaExpression)StripQuotes(expression.Arguments[1]);
            APredicateExpression predicate = Visit<APredicateExpression>(lambda.Body);
            return new WhereExpression(source, predicate);
        }

        private FieldExpression VisitField(MethodCallExpression expression)
        {
            // Resolve the field name
            ConstantExpression fieldNameExpression = expression.Arguments.FirstOrDefault() as ConstantExpression;
            if (!expression.Method.DeclaringType.IsAssignableFrom(typeof(Dictionary<string, object>)))
                throw new InvalidOperationException("The declaring type for a field expression must be a Dictionary<string, object>.");
            if (expression.Method.ReturnType != typeof(object))
                throw new InvalidOperationException("The return type for a field expression must be type of object.");
            if (expression.Arguments.Count != 1 || fieldNameExpression?.Type != typeof(string))
                throw new InvalidOperationException("The field name indexer for the field expression must contain exactly one string parameter.");

            // Resolve the table name
            MethodCallExpression source = expression.Object as MethodCallExpression;
            ConstantExpression tableNameExpression = source?.Arguments.FirstOrDefault() as ConstantExpression;
            if (source == null)
                throw new InvalidOperationException($"The table instance object could not be resolved for the field: {fieldNameExpression.Value.ToString()}");
            if (source.Method.Name != "get_Item")
                throw new NotSupportedException("Only an array indexer can be used to resolve a fields table name.");
            if (!source.Method.ReturnType.IsAssignableFrom(typeof(RecordItem)))
                throw new InvalidOperationException($"When mapping a field, the table name must map to a {nameof(RecordItem)}.");
            if (source.Arguments.Count != 1 || tableNameExpression?.Type != typeof(string))
                throw new InvalidOperationException("The table name indexer for the field expression must contain exactly one string parameter.");

            // Resolve the indexer values
            string tableName = tableNameExpression.Value as string;
            string fieldName = fieldNameExpression.Value as string;
            if (string.IsNullOrWhiteSpace(tableName))
                throw new InvalidOperationException("The table name cannot be empty.");
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new InvalidOperationException("The field name cannot be empty.");

            // TODO - Uhhh, how do we create a FieldExpresion here, the FieldExpressions of the source won't exist until this completes!
            //        Maybe a "FieldExpressionPromise" which just contains a Table and Field name.

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
