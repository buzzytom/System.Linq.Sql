namespace System.Linq.Sql.Samples
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SampleAttribute : Attribute
    {
        // ----- Properties ----- //

        public string Name { set; get; }

        public SampleSection Section { get; set; }
    }
}
