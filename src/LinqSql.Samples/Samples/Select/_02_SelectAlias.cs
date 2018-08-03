using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace System.Linq.Sql.Samples
{
    [Sample(Name = "Select Alias", Section = SampleSection.Select)]
    public class _02_SimpleSelect : ISample
    {
        public void Run()
        {
            using (BloggingContext context = new BloggingContext())
            {
                DbConnection connection = context.GetDbConnection();
                context.Database.Migrate();
                context.Seed();

                // Declare the fields that we want to query.
                // Note: Under normal usage of this library you would normally store the field definitions in the database.
                string[] fields = Blog.Fields;

                // Create the query from a DbConnection. The second argument defines an alias that will be used throughout the query.
                // Note: This is only an extension method, look at the Query definition to see how it creates the queryable.
                IQueryable<Record> query = context.Query("Blogs", "BlogsAlias", fields);

                // The declared alias (BlogsAlias) is now used in the place of the table name (Blogs).
                Record[] results = query
                    .Where(x => (int)x["BlogsAlias"]["BlogId"] == 1 || (int)x["BlogsAlias"]["BlogId"] == 2)
                    .ToArray();

                SamplesHelper.RenderQuery("select * from Blogs as BlogsAlias where BlogId = 1 or BlogId = 2");
                SamplesHelper.RenderRecords(results);
            }
        }
    }
}
