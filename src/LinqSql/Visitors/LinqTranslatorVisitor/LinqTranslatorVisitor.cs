using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="LinqTranslatorVisitor"/> translates an expression call to an sql expression tree.
    /// </summary>
    public partial class LinqTranslatorVisitor : ExpressionVisitor
    {
        private ASourceExpression[] sources = null;

        /// <summary>
        /// Initializes a new instance of <see cref="LinqTranslatorVisitor"/>.
        /// </summary>
        public LinqTranslatorVisitor()
        { }

        /// <summary>
        /// Translates the specified expression and its subtree to an <see cref="AExpression"/>.
        /// </summary>
        /// <param name="expression">The expression to modify.</param>
        /// <returns>The converted expression tree.</returns>
        public static Expression Translate(Expression expression)
        {
            LinqTranslatorVisitor visitor = new LinqTranslatorVisitor();
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

        internal T Visit<T>(Expression expression)
            where T : AExpression
        {
            T source = Visit(StripQuotes(expression)) as T;
            if (source == null)
                throw new NotSupportedException($"Could not convert the expression to an {typeof(T).Name}.");
            if (source is ASourceExpression)
                sources = new[] { source as ASourceExpression };
            return source;
        }

        /// <summary>
        /// Visits the System.Linq.Expressions.ConstantExpression and converts it into either a <see cref="NullExpression"/> if the value is null, <see cref="BooleanExpression"/> if the expression is a boolean, otherwise a <see cref="LiteralExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The specified expression converted to either a <see cref="NullExpression"/>, <see cref="BooleanExpression"/> or <see cref="LiteralExpression"/>.</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            // Detect null value
            object value = node.Value;
            if (value == null)
                return new NullExpression();

            // Detect boolean constants
            Type type = value.GetType();
            if (typeof(bool).IsAssignableFrom(type))
                return new BooleanExpression((bool)value);

            // Detect sub queries
            if (typeof(SqlQueryable).IsAssignableFrom(type))
                return ((SqlQueryable)value).Expression;

            return new LiteralExpression(node.Value);
        }

        /// <summary>
        /// Visits the children of the <see cref="UnaryExpression"/> converting the expression to an <see cref="AExpression"/> if the expression is a known convertible type.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, a <see cref="NotSupportedException"/> exception is thrown.</returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert)
                return Visit(node.Operand);
            else
                throw new NotSupportedException("That unary expression is not supported.");
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberExpression"/>. If the value of the member is an <see cref="SqlQueryable"/>, the queries <see cref="SqlQueryable.Expression"/> will be returned.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            // Get the member value
            object value = Expression
                .Lambda<Func<object>>(Expression.Convert(node, typeof(object)))
                .Compile()
                .Invoke();

            // Create a constant expression from the value
            // This allows VisitConstant to define decoding logic
            return VisitConstant(Expression.Constant(value));
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

        /// <summary>
        /// Visits the children of the <see cref="MethodCallExpression"/>, translating the expression to an <see cref="ASourceExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The specified expression converted to an <see cref="ASourceExpression"/>; otherwise a thrown exception.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "Contains":
                    return VisitContains(node);
                case "Count":
                    return VisitCount(node);
                case "Average":
                    return VisitAggregate(node, AggregateFunction.Average);
                case "Sum":
                    return VisitAggregate(node, AggregateFunction.Sum);
                case "Min":
                    return VisitAggregate(node, AggregateFunction.Min);
                case "Max":
                    return VisitAggregate(node, AggregateFunction.Max);
                case "get_Item":
                    return VisitField(node);
                case "OrderBy":
                case "OrderByDescending":
                case "ThenBy":
                case "ThenByDescending":
                    return VisitOrderBy(node);
                case "Where":
                    return VisitWhere(node);
                case "Join":
                    return VisitJoin(node);
                case "Skip":
                    return VisitSkip(node);
                case "Take":
                    return VisitTake(node);
                default:
                    throw new MethodTranslationException(node.Method);
            }
        }

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
                expression = ((UnaryExpression)expression).Operand;
            return expression;
        }

        private static bool IsDeclaring<T1>(MethodCallExpression expression)
        {
            return IsDeclaring(expression, typeof(T1));
        }

        private static bool IsDeclaring<T1, T2>(MethodCallExpression expression)
        {
            return IsDeclaring(expression, typeof(T1), typeof(T2));
        }

        private static bool IsDeclaring<T1, T2, T3>(MethodCallExpression expression)
        {
            return IsDeclaring(expression, typeof(T1), typeof(T2), typeof(T3));
        }

        private static bool IsDeclaring(MethodCallExpression expression, params Type[] types)
        {
            Type type = expression?.Method?.DeclaringType;
            if (type == null)
                throw new ArgumentException("The specified expression does not define a DeclaringType.", nameof(expression));
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            return types.Contains(type);
        }
    }
}
