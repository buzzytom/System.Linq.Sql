using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace System.Linq.Sql.Samples
{
    [Sample(Name = "Select Ordering", Section = SampleSection.Select)]
    public class _03_SelectOrdering : ISample
    {
        public void Run()
        {
            using (BloggingContext context = new BloggingContext())
            {
                DbConnection connection = context.GetDbConnection();
                context.Database.Migrate();
                context.Seed();

                // The Linq extensions OrderBy, OrderByDescending, ThenBy and ThenByDescending are supported
                Record[] results = context
                    .Query("Posts", Post.Fields)
                    .OrderBy(x => x["Posts"]["PostId"])
                        .ThenByDescending(x => x["Posts"]["Content"])
                    .ToArray();

                SamplesHelper.RenderQuery("select * from Posts order by PostId asc, Content desc");
                SamplesHelper.RenderRecords(results);
            }
        }
    }
}
