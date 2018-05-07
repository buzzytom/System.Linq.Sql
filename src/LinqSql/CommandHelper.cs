using System.Data.Common;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="CommandHelper"/> defines extension methods <see cref="DbCommand"/> instances.
    /// </summary>
    public static class CommandHelper
    {
        /// <summary>
        /// Adds the specified <paramref name="value"/> as the parameter for the specified <paramref name="identifier"/> of the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter to.</param>
        /// <param name="identifier">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public static void AddParameter(this DbCommand command, string identifier, object value)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Cannot be null or whitespace.", nameof(identifier));

            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = identifier;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}
