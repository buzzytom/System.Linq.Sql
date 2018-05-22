using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="SqlTranslatorVisitor"/> translates an expression call to an sql expression tree.
    /// </summary>
    public class SqlTranslatorVisitor : ExpressionVisitor
    {
        private ASourceExpression[] sources = null;

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
            if (source is ASourceExpression)
                sources = new[] { source as ASourceExpression };
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
                case "Contains":
                    return VisitContains(node);
                case "get_Item":
                    return VisitField(node);
                case "Where":
                    return VisitWhere(node);
                case "Join":
                    return VisitJoin(node);
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

        private ContainsExpression VisitContains(MethodCallExpression expression)
        {
            // Handle extension methods defined by Linqs
            if (expression.Method.DeclaringType == typeof(Enumerable) || expression.Method.DeclaringType == typeof(Queryable))
            {
                AExpression values = Visit<AExpression>(expression.Arguments[0]);
                AExpression value = Visit<AExpression>(expression.Arguments[1]);
                return new ContainsExpression(values, value);
            }

            // Handle custom extension method
            if (expression.Method.DeclaringType == typeof(SqlQueryableHelper))
            {
                // Get value expression first, because the source will change to the subquery making the value out of scope
                AExpression value = Visit<AExpression>(expression.Arguments[2]);
                
                // Evaluate the subquery expressions
                ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);
                LambdaExpression fieldLambda = (LambdaExpression)StripQuotes(expression.Arguments[1]);
                FieldExpression field = Visit<FieldExpression>(fieldLambda.Body);

                // Create the expression
                return new ContainsExpression(new ScalarExpression(source, field), value);
            }

            throw new InvalidOperationException($"The {expression.Method.DeclaringType.Name} implementation of Contains is not supported by the translator.");
        }

        private WhereExpression VisitWhere(MethodCallExpression expression)
        {
            // Handle the default Queryable extension Where
            if (expression.Method.DeclaringType == typeof(Queryable))
            {
                ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(expression.Arguments[1]);
                APredicateExpression predicate = Visit<APredicateExpression>(lambda.Body);
                return new WhereExpression(source, predicate);
            }

            throw new InvalidOperationException($"The {expression.Method.DeclaringType.Name} implementation of Where is not supported by the translator.");
        }

        private JoinExpression VisitJoin(MethodCallExpression expression)
        {
            // Handle the default Queryable extension Join
            if (expression.Method.DeclaringType == typeof(Queryable))
            {
                // Resolve the sources
                ASourceExpression outer = Visit<ASourceExpression>(expression.Arguments[0]);
                ASourceExpression inner = Visit<ASourceExpression>(expression.Arguments[1]);

                // Set the active expressions (so fields calls can find their expression)
                sources = new[] { outer, inner };

                // Create the predicate
                LambdaExpression outerLambda = (LambdaExpression)StripQuotes(expression.Arguments[2]);
                LambdaExpression innerLambda = (LambdaExpression)StripQuotes(expression.Arguments[3]);
                FieldExpression outerField = Visit<FieldExpression>(outerLambda.Body);
                FieldExpression innerField = Visit<FieldExpression>(innerLambda.Body);
                APredicateExpression predicate = new CompositeExpression(outerField, innerField, CompositeOperator.Equal);

                // Decode the result selector
                IEnumerable<AFieldExpression> fields = DecodeJoinSelector(expression.Arguments[4], outer.Fields, inner.Fields);

                // Create the expression
                return new JoinExpression(outer, inner, predicate, fields, JoinType.Inner);
            }

            // Handle the default SqlQueryableHelper extension Join
            if (expression.Method.DeclaringType == typeof(SqlQueryableHelper))
            {
                // Resolve the sources
                ASourceExpression outer = Visit<ASourceExpression>(expression.Arguments[0]);
                ASourceExpression inner = Visit<ASourceExpression>(expression.Arguments[1]);

                // Set the active expressions (so fields calls can find their expression)
                sources = new[] { outer, inner };

                // Create the predicate
                LambdaExpression predicateLambda = (LambdaExpression)StripQuotes(expression.Arguments[2]);
                APredicateExpression predicate = Visit<APredicateExpression>(predicateLambda.Body);

                // Decode the result selector
                IEnumerable<AFieldExpression> fields = DecodeJoinSelector(expression.Arguments[3], outer.Fields, inner.Fields);

                // Resolve the join type
                ConstantExpression joinType = (ConstantExpression)expression.Arguments[4];

                // Create the expression
                return new JoinExpression(outer, inner, predicate, fields, (JoinType)joinType.Value);
            }

            throw new InvalidOperationException($"The {expression.Method.DeclaringType.Name} implementation of Join is not supported by the translator.");
        }

        private IEnumerable<AFieldExpression> DecodeJoinSelector(Expression expression, FieldExpressions outer, FieldExpressions inner)
        {
            // Get the associated lambda
            LambdaExpression lambda = StripQuotes(expression) as LambdaExpression;

            // Decode paramter body
            ParameterExpression parameter = lambda.Body as ParameterExpression;
            if (parameter != null)
            {
                if (parameter == lambda.Parameters[0])
                    return outer;
                else if (parameter == lambda.Parameters[1])
                    return inner;

                throw new InvalidOperationException($"The Join result selector does not recognise the specified '{parameter}' parameter. When using a paramter it must be one of the result selector paramter list.");
            }

            // Decode binary selector (It should always evaluate to all fields)
            BinaryExpression binary = lambda.Body as BinaryExpression;
            if (binary != null)
            {
                // Check the correct operator is being used
                if (binary.NodeType != ExpressionType.Or)
                    throw new NotSupportedException("The only supported binary operator of an Sql Join call is Or: '|'.");

                // Check the binary expressions are the paramters specified in the expression
                if (binary.Left != lambda.Parameters[0] && binary.Left != lambda.Parameters[1])
                    throw new NotSupportedException("The left of the '|' operator in a Join result selector must be a parameter of the selector expression.");
                if (binary.Right != lambda.Parameters[0] && binary.Right != lambda.Parameters[1])
                    throw new NotSupportedException("The right of the '|' operator in a Join result selector must be a parameter of the selector expression.");

                // Resolve select all
                if ((binary.Left == lambda.Parameters[0] && binary.Right == lambda.Parameters[1]) ||
                    (binary.Left == lambda.Parameters[1] && binary.Right == lambda.Parameters[0]))
                    return null;

                throw new InvalidOperationException("The Join result selector should not select the same parameter more than once. e.g. '(o, i) => i | i' should be replaced with '(o, i) => i'.");
            }

            throw new NotSupportedException($"The Join result selctor does not know how to translate '{expression}' to SQL. The supported expressions are: '(o, i) => o', '(o, i) => i', '(o, i) => o | i' and '(o, i) => i | o'.");
        }

        private AFieldExpression VisitField(MethodCallExpression expression)
        {
            if (sources == null)
                throw new InvalidOperationException($"A field can only be visited if an {nameof(ASourceExpression)} has previously been visited.");

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

            // Get the field from the current source
            AFieldExpression found = sources?
                .SelectMany(x => x.Fields)
                .FirstOrDefault(x => x.TableName == tableName && x.FieldName == fieldName);
            if (found == null)
                throw new KeyNotFoundException($"The field [{tableName}].[{fieldName}] could not be found on the current source expression.");

            return found;
        }

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
                expression = ((UnaryExpression)expression).Operand;
            return expression;
        }
    }
}
