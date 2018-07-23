using System.Linq.Expressions;

namespace System.Linq.Sql
{
    public partial class LinqTranslatorVisitor
    {
        private SelectExpression VisitSkip(MethodCallExpression expression)
        {
            if (IsDeclaring(expression, typeof(Queryable), typeof(Enumerable)))
            {
                ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);
                int count = (int)((ConstantExpression)expression.Arguments[1]).Value;
                return new SelectExpression(source, source.Fields, -1, count);
            }

            throw new MethodTranslationException(expression.Method);
        }

        private SelectExpression VisitTake(MethodCallExpression expression)
        {
            if (IsDeclaring(expression, typeof(Queryable), typeof(Enumerable)))
            {
                ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);
                int count = (int)((ConstantExpression)expression.Arguments[1]).Value;
                return new SelectExpression(source, source.Fields, count, 0);
            }

            throw new MethodTranslationException(expression.Method);
        }
    }
}
