using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Sql
{
    public partial class LinqTranslatorVisitor
    {
        private SelectExpression VisitOrderBy(MethodCallExpression expression)
        {
            MethodInfo method = expression.Method;
            if (IsDeclaring(expression, typeof(Queryable), typeof(Enumerable), typeof(SqlQueryableHelper)))
            {
                // Resolve the source
                ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);

                // Resolve the optional selector
                FieldExpression field = source.Fields.First();
                if (expression.Arguments.Count > 1)
                {
                    LambdaExpression lambda = (LambdaExpression)StripQuotes(expression.Arguments[1]);
                    field = Visit<FieldExpression>(lambda.Body);
                }

                // Decode the direction
                OrderType direction = method.Name.EndsWith("Descending") ? OrderType.Descending : OrderType.Ascending;

                // Handle an existing select expression
                if (source is SelectExpression select)
                {
                    if (method.Name.StartsWith("ThenBy") && !select.Orderings.Any())
                        throw new InvalidOperationException($"{method.Name} can only be applied to an ordered sequence.");
                    if (method.Name.StartsWith("OrderBy") && select.Orderings.Any())
                        throw new InvalidOperationException($"{method.Name} can only be applied to an unordered sequence.");

                    // Clone and modify the select expression
                    IEnumerable<FieldExpression> fields = select.Fields.Select(x => x.SourceExpression);
                    IEnumerable<Ordering> orderings = select.Orderings.Concat(new[] { new Ordering(field.SourceExpression, direction) });
                    return new SelectExpression(select.Source, fields, select.Take, select.Skip, orderings);
                }

                // Create the expression
                if (method.Name.StartsWith("ThenBy"))
                    throw new InvalidOperationException($"{method.Name} can only be applied to an ordered sequence.");
                return new SelectExpression(source, orderings: new[] { new Ordering(field, direction) });
            }

            throw new MethodTranslationException(expression.Method);
        }
    }
}
