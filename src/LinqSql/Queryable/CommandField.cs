namespace System.Linq.Sql
{
#if DEBUG
    public
#else
    internal
#endif
    struct CommandField
    {
        private readonly string field;
        private readonly int ordinal;
        private readonly string table;

        public CommandField(string table, string field, int ordinal)
        {
            this.table = table;
            this.field = field;
            this.ordinal = ordinal;
        }

        // ----- Properties ----- //

        public string Table => table;

        public string FieldName => field;

        public int Ordinal => ordinal;
    }
}
