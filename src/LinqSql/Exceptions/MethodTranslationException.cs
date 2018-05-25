using System.Reflection;

namespace System.Linq.Sql
{
    /// <summary>
    /// Represents an error whilst converting a method in an expression tree to an sql expression tree.
    /// </summary>
    public class MethodTranslationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodTranslationException"/> class with the specified <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The method that could not be translated.</param>
        public MethodTranslationException(MethodInfo method)
            : base($"The {method?.DeclaringType.Name ?? throw new ArgumentNullException(nameof(method))} implementation of {method.Name} is not known by the translator.")
        { }
    }
}
