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
            // Get the associated method
            MethodInfo method = node.Method;
            if (method.DeclaringType != typeof(Queryable))
                throw new InvalidOperationException($"Cannot translate the method '{method.Name}' unless the source is a System.Linq.Queryable instance.");

            // Decode specific method node handling
            if (method.Name == "Where")
            {
                ASourceExpression source = Visit<ASourceExpression>(node.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                APredicateExpression predicate = Visit<APredicateExpression>(lambda.Body);
                return new WhereExpression(source, predicate);
            }

            throw new NotSupportedException($"Cannot translate the method '{method.Name}' because it's not known by the sql translator.");
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

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
                expression = ((UnaryExpression)expression).Operand;
            return expression;
        }
    }
}
