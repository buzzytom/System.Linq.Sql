using System.Collections.Generic;

namespace System.Linq.Sql.Samples
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
                exit = RenderSectionSelection();
        }

        private static bool RenderSectionSelection()
        {
            // Render the options
            Console.Clear();
            Console.WriteLine("===== System.Linq.Sql Samples =====");
            Console.WriteLine("These samples demonstrate some of the common usages of this library.");
            SamplesHelper.RenderOptions<SampleSection>();

            // Read the desired section
            SampleSection section = SamplesHelper.ReadInRange<SampleSection>();
            if (section == SampleSection.Exit)
                return true;

            // Render the section
            RenderSampleSelection(section);

            return false;
        }

        private static void RenderSampleSelection(SampleSection section)
        {
            // Get all the samples exposed by the current assembly
            KeyValuePair<SampleAttribute, Type>[] samples = SamplesHelper
                .GetSectionSamples(section)
                .ToArray();

            // Calculate the selectable options
            string[] options = samples
                .Select(x => x.Key.Name)
                .Concat(new[] { "Cancel" })
                .ToArray();

            // Read in the sample option
            Console.Clear();
            Console.WriteLine($"===== {section} Samples =====");
            SamplesHelper.RenderOptions(options);
            int index =  SamplesHelper.ReadIntInRange(1, options.Length) - 1;

            // Resolve the action
            if (index < options.Length - 1)
                RenderSample(samples[index].Key, samples[index].Value);
        }

        private static void RenderSample(SampleAttribute sample, Type type)
        {
            Console.Clear();
            Console.WriteLine($"===== {sample.Name} =====");
            ISample instance = Activator.CreateInstance(type) as ISample;
            if (instance == null)
                Console.WriteLine("Could not create an instance of the sample.");
            instance.Run();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
