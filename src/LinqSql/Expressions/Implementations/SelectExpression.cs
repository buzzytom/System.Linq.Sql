using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// An enumeration of the order directions supported by sql.
    /// </summary>
    public enum OrderType
    {
        /// <summary>Values will be sorted such that the next item is greater than or equal to its previous value.</summary>
        Ascending,
        /// <summary>Values will be sorted such that the next item is less than or equal to its previous value.</summary>
        Descending
    }

    /// <summary>
    /// <see cref="Ordering"/> represents the ordering direction of a single column.
    /// </summary>
    public class Ordering
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Ordering"/> with the specified field and order direction.
        /// </summary>
        /// <param name="field">The field to order.</param>
        /// <param name="type">The direction to order the column.</param>
        public Ordering(FieldExpression field, OrderType type)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            Field = field;
            OrderType = type;
        }

        // ----- Properties ----- //

        /// <summary>Gets the Field of this object instance.</summary>
        public FieldExpression Field { get; }

        /// <summary>Gets the OrderType of this object instance.</summary>
        public OrderType OrderType { get; }
    }

    /// <summary>
    /// Represents the field selection on a source expression.
    /// </summary>
    public class SelectExpression : ASourceExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SelectExpression"/> selecting all the fields from the specified source.
        /// </summary>
        /// <param name="source">The source expression to select from.</param>
        /// <param name="take">The number of fields to take from the source. Values less than zero indicate all rows.</param>
        /// <param name="skip">The number of fields to ignore on the source before reading rows.</param>
        /// <param name="orderings">The optional sorting of the selected rows.</param>
        /// <exception cref="ArgumentNullException">the source <see cref="ASourceExpression.Fields"/> property is null.</exception>
        /// <exception cref="ArgumentException">the source <see cref="ASourceExpression.Fields" /> property contains no elements.</exception>
        /// <exception cref="KeyNotFoundException">An ordering key could not be found in the sources <see cref="ASourceExpression.Fields"/> property.</exception>
        public SelectExpression(ASourceExpression source, int take = -1, int skip = 0, IEnumerable<Ordering> orderings = null)
            : this(source, source?.Fields, take, skip, orderings)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="SelectExpression"/> selecting the specified fields from the source.
        /// </summary>
        /// <param name="source">The source expression to select from.</param>
        /// <param name="fields">The fields to select from the source.</param>
        /// <param name="take">The number of fields to take from the source. Values less than zero indicate all rows.</param>
        /// <param name="skip">The number of fields to ignore on the source before reading rows.</param>
        /// <param name="orderings">The optional sorting of the selected rows.</param>
        /// <exception cref="ArgumentNullException">fields is null.</exception>
        /// <exception cref="ArgumentException">fields contains no elements.</exception>
        /// <exception cref="KeyNotFoundException">An ordering key could not be found in fields.</exception>
        public SelectExpression(ASourceExpression source, IEnumerable<FieldExpression> fields, int take = -1, int skip = 0, IEnumerable<Ordering> orderings = null)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (!fields.Any())
                throw new ArgumentException("There must be at least one field specified in a select query.", nameof(fields));

            Source = source;
            Fields = new FieldExpressions(this, fields);
            Expressions = new[] { source };
            Take = take;
            Skip = skip;

            Orderings = orderings?
                .Select(x =>
                {
                    FieldExpression field = Fields.FirstOrDefault(y => y.SourceExpression == x.Field);
                    if (field == null)
                        throw new KeyNotFoundException($"The ordering field '[{x.Field.TableName}].[{x.Field.FieldName}]' could not be found in the fields parameter.");
                    return x;
                })
                .ToArray();
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitSelect(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets inner source of the expression.</summary>
        public ASourceExpression Source { get; }

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public override FieldExpressions Fields { get; }

        /// <summary>Gets the child expressions of this source.</summary>
        public override IEnumerable<ASourceExpression> Expressions { get; }

        /// <summary>Gets the number of rows that will be skipped on the source expression.</summary>
        public int Skip { get; } = 0;

        /// <summary>Gets the number of rows that will be taken from the source expression.</summary>
        public int Take { get; } = -1;

        /// <summary>Gets the orderings that will be applied to the source expression.</summary>
        public IEnumerable<Ordering> Orderings { get; }
    }
}
