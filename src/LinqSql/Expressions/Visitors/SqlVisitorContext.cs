using System.Collections.Generic;

namespace System.Linq.Sql.Expressions
{
    /// <summary>
    /// Defines a shared data store for use in evaluation of the query syntax tree. This class can be used to track parameter input values for the query and previously evaluated <see cref="ASourceExpression"/> items. 
    /// </summary>
    public class SqlVisitorContext
    {
        private Dictionary<string, object> parameters = new Dictionary<string, object>();
        private int parameter = 0;

        private Dictionary<ASourceExpression, string> sources = new Dictionary<ASourceExpression, string>();
        private int source = 0;

        /// <summary>
        /// Removes all cached parameters and sources from the <see cref="SqlVisitorContext"/> instance.
        /// </summary>
        public void Clear()
        {
            parameters.Clear();
            sources.Clear();
            parameter = 0;
            source = 0;
        }

        /// <summary>
        /// Creates an input parameter for the query with a unique name. This method avoids SQL injection.
        /// </summary>
        /// <param name="value">The object that represents the input value.</param>
        /// <returns>The unique name for the created input parameter.</returns>
        public string CreateParameter(object value)
        {
            string name = "p" + parameter++;
            parameters[name] = value;
            return name;
        }

        /// <summary>
        /// Creates a unique name for an <see cref="IQueryNode"/>. Identical input nodes that have the same reference will always produce the same name.
        /// </summary>
        /// <param name="node">The input node to get an identifiable name for.</param>
        /// <returns>A unique name for the specified <see cref="IQueryNode"/> in the format "t{index}".</returns>
        public string GetSource(ASourceExpression node)
        {
            if (sources.TryGetValue(node, out string name))
                return name;
            else
            {
                name = "t" + source++;
                sources[node] = name;
                return name;
            }
        }

        // ----- Properties ----- //

        /// <summary>Gets the Parameters value of this object.</summary>
        /// <remarks>This is a collection of all the query input parameters that needed to be passed to the SQL execution engine.</remarks>
        public Dictionary<string, object> Parameters => parameters;
    }
}
