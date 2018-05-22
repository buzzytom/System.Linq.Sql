using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Sql
{
    public class FieldExpressions : IEnumerable<AFieldExpression>
    {
        private int next = 0;
        private Dictionary<AFieldExpression, string> fields = new Dictionary<AFieldExpression, string>();

        /// <summary>
        /// Initializes a new empty instance of <see cref="FieldExpressions"/>.
        /// </summary>
        public FieldExpressions()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="FieldExpressions"/> with the specified table and fields names.
        /// </summary>
        /// <param name="source">The expression which the fields belong to.</param>
        /// <param name="table">The name of the table all the fields are in.</param>
        /// <param name="fields">The fields to add.</param>
        public FieldExpressions(ASourceExpression source, string table, IEnumerable<string> fields)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be null or whitespace.", nameof(table));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (fields.Any(x => string.IsNullOrWhiteSpace(x)))
                throw new ArgumentException("One or more field was whitespace.", nameof(fields));

            foreach (string field in fields)
                Add(new FieldExpression(source, table, field));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FieldExpressions"/> with the specified source and fields.
        /// </summary>
        /// <param name="source">The expression which the fields belong to.</param>
        /// <param name="fields">The fields to add.</param>
        public FieldExpressions(ASourceExpression source, IEnumerable<AFieldExpression> fields)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (fields.Any(x => x == null))
                throw new ArgumentException("One or more field was null.", nameof(fields));

            foreach (AFieldExpression field in fields)
                Add(new FieldExpression(source, field.TableName, field.FieldName, field));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FieldExpression"/> with the specified fields.
        /// </summary>
        /// <param name="fields">The fields to add.</param>
        public FieldExpressions(IEnumerable<AFieldExpression> fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            foreach (AFieldExpression field in fields)
                Add(field);
        }

#if DEBUG
        public
#else
        private
#endif
        string Add(AFieldExpression field)
        {
            if (fields.TryGetValue(field, out string key))
                return key;
            else
            {
                key = $"f{next}";
                fields[field] = key;
                next++;
                return key;
            }
        }

        /// <summary>
        /// Gets the key associated with a given field.
        /// </summary>
        /// <param name="field">The field to get the key for.</param>
        /// <returns>The key assigned to the field</returns>
        /// <remarks>This method will throw an <see cref="KeyNotFoundException"/> if the field does not exist.</remarks>
        public string GetKey(AFieldExpression field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            else if (fields.TryGetValue(field, out string key))
                return key;
            else
                throw new KeyNotFoundException("The field could not be found in the collection.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<AFieldExpression> GetEnumerator()
        {
            return fields.Keys.GetEnumerator();
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
