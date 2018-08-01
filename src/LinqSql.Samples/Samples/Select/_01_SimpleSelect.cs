using System.Data.Common;
using Microsoft.EntityFrameworkCore;

using System.Linq.Sql.Sqlite;

namespace System.Linq.Sql.Samples
{
    [Sample(Name = "Simple Select", Section = SampleSection.Select)]
    public class _01_SimpleSelect : ISample
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

                // Create the query from a DbConnection.
                // Note: We use the connection provided by the Entity Framework DbContext. There is no requirement to use a connection provided by Entity Framework.
                //       If you are using an Entity Framework DbContext in your code, we recommend making a method on your context exposing the provider.
                //       We will only be explicit in this sample.
                IQueryable<Record> query = new SqliteQueryable(connection, "Blogs", fields);

                // Traditional linq syntax can be used to predicate your queries
                // When comparing a field value you must specify the table and field with this square bracket style:
                // ["table"]["field"]
                // Note: The casts are required for the comparisions to be valid C#
                query = query.Where(x => (int)x["Blogs"]["BlogId"] == 1 || (int)x["Blogs"]["BlogId"] == 2);

                // Executing the query can be achieved with methods like ToArray, ToList, FirstOrDefault etc.
                // Note: Helper methods exist to flatten results which we will cover in other samples
                Record[] results = query.ToArray();

                SamplesHelper.RenderQuery("select * from Blogs where BlogId = 1 or BlogId = 2");
                SamplesHelper.RenderRecords(results);
            }
        }
    }
}
