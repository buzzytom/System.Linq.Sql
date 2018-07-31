using System.Collections.Generic;

namespace System.Linq.Sql.Samples
{
    internal static class SamplesHelper
    {
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
                .OrderBy(x => x.Key.Name);
        }

        private static SampleAttribute GetSampleAttribute(this Type type)
        {
            return type?
                .GetCustomAttributes(typeof(SampleAttribute), false)
                .FirstOrDefault() as SampleAttribute;
        }
    }
}
