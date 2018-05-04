using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Sql.Expressions
{
    public class FieldExpressions : IEnumerable<FieldExpression>
    {
        private int next = 0;
        private Dictionary<FieldExpression, string> fields = new Dictionary<FieldExpression, string>();

        /// <summary>
        /// Initializes a new empty instance of <see cref="FieldExpressions"/>.
        /// </summary>
        public FieldExpressions()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="FieldExpression"/> with the specified table and fields names.
        /// </summary>
        /// <param name="table">The name of the table all the fields are in.</param>
        /// <param name="fields">The fields to add.</param>
        public FieldExpressions(string table, IEnumerable<string> fields)
        {
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be null or whitespace.", nameof(table));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            foreach (string field in fields)
                Add(new FieldExpression(table, field));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FieldExpression"/> with the specified fields.
        /// </summary>
        /// <param name="fields">The fields to add.</param>
        public FieldExpressions(IEnumerable<FieldExpression> fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            foreach (FieldExpression field in fields)
                Add(field);
        }

        /// <summary>
        /// Adds a <see cref="FieldExpression"/> to the collection.
        /// </summary>
        /// <param name="field">The field to add.</param>
        /// <returns>The alias identifier of the added field.</returns>
        public string Add(FieldExpression field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

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
        /// <remarks>Unlike <see cref="Add(FieldExpression)"/>, this method will throw an <see cref="KeyNotFoundException"/> if the field does not exist.</remarks>
        public string GetKey(FieldExpression field)
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
        public IEnumerator<FieldExpression> GetEnumerator()
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
