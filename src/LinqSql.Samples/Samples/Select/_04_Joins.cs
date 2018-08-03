using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace System.Linq.Sql.Samples
{
    [Sample(Name = "Joins", Section = SampleSection.Select)]
    public class _04_Joins : ISample
    {
        public void Run()
        {
            using (BloggingContext context = new BloggingContext())
            {
                DbConnection connection = context.GetDbConnection();
                context.Database.Migrate();
                context.Seed();

                // Create the inner query to join.
                IQueryable<Record> posts = context.Query("Posts", Post.Fields);

                // Query Blogs and left join Posts onto it.
                // Note: This query demonstrates the overloaded Linq Join method. The normal Linq Join methods are also compatible.
                Record[] results = context
                    .Query("Blogs", Blog.Fields)
                    .Join(posts,                                                            // The inner sequence to join
                          (x, y) => (int)x["Posts"]["BlogId"] == (int)x["Blogs"]["BlogId"], // The correlation predicate of the join
                          (outer, inner) => outer | inner,                                  // Allows you to select fields from the outer, inner or both
                          JoinType.Left)                                                    // The type of join
                    .ToArray();
                
                SamplesHelper.RenderQuery("select * from Blogs left join Posts on Posts.BlogId = Blogs.BlogId");
                SamplesHelper.RenderRecords(results);
            }
        }
    }
}
