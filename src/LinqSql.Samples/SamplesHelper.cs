using System.Collections.Generic;

namespace System.Linq.Sql.Samples
{
    internal static class SamplesHelper
    {
        private static string[] keywords = { "select", "from", "where", "limit", "or", "and", "as" };

        public static T ReadInRange<T>()
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"{nameof(T)} must be an enumerated type.");

            IEnumerable<int> values = Enum
                .GetValues(typeof(T))
                .Cast<int>();
            return (T)(object)ReadIntInRange(values.Min(), values.Max());
        }

        public static int ReadIntInRange(int lower, int upper)
        {
            while (true)
            {
                Console.WriteLine($"Enter option {lower} to {upper}:");
                if (int.TryParse(Console.ReadLine(), out int index) && index >= lower && index <= upper)
                    return index;
            }
        }

        public static void RenderOptions<T>()
               where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"{nameof(T)} must be an enumerated type.");

            IEnumerable<string> options = Enum
                .GetValues(typeof(T))
                .Cast<T>()
                .OrderBy(x => Convert.ToInt32(x))
                .Select(x => x.ToString().Replace('_', ' '));
            RenderOptions(options);
        }

        public static void RenderOptions(IEnumerable<string> options)
        {
            int index = 1;
            foreach (string option in options)
            {
                Console.WriteLine($"{index}. {option}");
                index++;
            }
        }

        public static IOrderedEnumerable<KeyValuePair<SampleAttribute, Type>> GetSectionSamples(SampleSection section)
        {
            return typeof(SampleSection).Assembly
                .GetTypes()
                .Where(x => typeof(ISample).IsAssignableFrom(x))
                .Select(x => new
                {
                    Attribute = x.GetSampleAttribute(),
                    Type = x
                })
                .Where(x => x.Attribute != null)
                .ToDictionary(x => x.Attribute, x => x.Type)
                .Where(x => x.Key?.Section == section)
                .OrderBy(x => x.Value.Name);
        }

        private static SampleAttribute GetSampleAttribute(this Type type)
        {
            return type?
                .GetCustomAttributes(typeof(SampleAttribute), false)
                .FirstOrDefault() as SampleAttribute;
        }

        public static void RenderQuery(string sql)
        {
            // Split the sql query into tokens
            string[] tokens = sql.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in tokens)
            {
                // Determine the colour of the token
                if (keywords.Any(x => string.Equals(token, x, StringComparison.OrdinalIgnoreCase)))
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else
                    Console.ForegroundColor = ConsoleColor.White;

                // Render the token
                Console.Write(token + " ");
            }
            Console.Write(Environment.NewLine);

            // Reset the console colour
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void RenderRecords(IEnumerable<Record> records, int columnWidth = 16)
        {
            if (records.Count() == 0)
                Console.WriteLine("No Records.");
            else
            {
                // Get the first records headings
                IEnumerable<string> headings = records
                    .First()
                    .Values
                    .SelectMany(x => x.Keys);
                int columns = headings.Count();

                // Render the headings
                RenderDivider(columns, columnWidth);
                RenderRow(headings, columnWidth);
                RenderDivider(columns, columnWidth);

                // Render cells
                foreach (Record record in records)
                {
                    IEnumerable<string> values = record.Values
                        .SelectMany(x => x.Values)
                        .Select(x => x.ToString());
                    RenderRow(values, columnWidth);
                }

                // Render Footer
                RenderDivider(columns, columnWidth);
            }
        }

        private static void RenderDivider(int columns, int columnWidth)
        {
            Console.Write("|".PadRight(columns * (columnWidth + 1), '-') + "|" + Environment.NewLine);
        }

        private static void RenderRow(IEnumerable<string> values, int columnWidth)
        {
            Console.Write("|");
            foreach (string value in values)
                Console.Write(String.Format("{0," + columnWidth + "}|", value.Truncate(columnWidth)));
            Console.Write(Environment.NewLine);
        }

        private static string Truncate(this string value, int length)
        {
            if (value.Length > length)
                return value.Substring(0, length);
            else
                return value;
        }
    }
}
