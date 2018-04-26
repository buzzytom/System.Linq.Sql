using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LinqSql.Expressions
{
    public class FieldExpressions : IEnumerable<FieldExpression>
    {
        private int next = 0;
        private LinkedList<FieldExpression> expressions = new LinkedList<FieldExpression>();

        public FieldExpressions()
        {
        }

        public FieldExpressions(ASourceExpression source, IEnumerable<string> fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (string field in fields)
                Add(source, field);
        }

        public void Add(ASourceExpression source, string field)
        {
            FieldExpression found = source.Fields.FirstOrDefault(x => x.Alias == field);
            if (found == null)
                throw new Exception($"The field '{field}' could not be found in the source.");

            expressions.AddLast(new FieldExpression(source, field, $"f{next}"));
            next++;
        }

        public IEnumerator<FieldExpression> GetEnumerator()
        {
            return expressions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return expressions.GetEnumerator();
        }
    }
}
