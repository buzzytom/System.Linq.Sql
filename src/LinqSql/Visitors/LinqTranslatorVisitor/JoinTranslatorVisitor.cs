using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Sql
{
    public partial class LinqTranslatorVisitor
    {
        public JoinExpression VisitJoin(MethodCallExpression expression)
        {
            // Handle the default Queryable extension Join
            if (IsDeclaring(expression, typeof(Queryable), typeof(Enumerable)))
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
                IEnumerable<FieldExpression> fields = DecodeJoinSelector(expression.Arguments[4], outer.Fields, inner.Fields);

                // Create the expression
                return new JoinExpression(outer, inner, predicate, fields, JoinType.Inner);
            }

            // Handle the default SqlQueryableHelper extension Join
            if (IsDeclaring(expression, typeof(SqlQueryableHelper)))
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
                IEnumerable<FieldExpression> fields = DecodeJoinSelector(expression.Arguments[3], outer.Fields, inner.Fields);

                // Resolve the join type
                ConstantExpression joinType = (ConstantExpression)expression.Arguments[4];

                // Create the expression
                return new JoinExpression(outer, inner, predicate, fields, (JoinType)joinType.Value);
            }

            throw new MethodTranslationException(expression.Method);
        }

        private IEnumerable<FieldExpression> DecodeJoinSelector(Expression expression, FieldExpressions outer, FieldExpressions inner)
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
    }
}
