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

        /// <summary>
        /// Initializes a new empty instance of <see cref="FieldExpressions"/>.
        /// </summary>
        public FieldExpressions()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FieldExpression"/> with the specified fields. The generated <see cref="FieldExpression"/> instances all share the same specified source.
        /// </summary>
        /// <param name="source">The source to use for each generated <see cref="FieldExpression"/>.</param>
        /// <param name="fields">The names of the fields on the source.</param>
        public FieldExpressions(ASourceExpression source, IEnumerable<string> fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (string field in fields)
                Add(source, field);
        }

        /// <summary>
        /// Adds a new <see cref="FieldExpression"/> to the collection using the specified source and field name.
        /// </summary>
        /// <param name="source">The source which the field exists on.</param>
        /// <param name="field">The name of the field on the source.</param>
        public void Add(ASourceExpression source, string field)
        {
            FieldExpression found = source.Fields.FirstOrDefault(x => x.Alias == field);
            if (found == null)
                throw new Exception($"The field '{field}' could not be found in the source.");

            expressions.AddLast(new FieldExpression(source, field, $"f{next}"));
            next++;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<FieldExpression> GetEnumerator()
        {
            return expressions.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
