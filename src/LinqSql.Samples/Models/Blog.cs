using System.Collections.Generic;

namespace System.Linq.Sql.Samples
{
    public class Blog
    {
        public int BlogId { get; set; }

        public string Url { get; set; }

        public int Rating { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>();

        public static string[] Fields { get; } =
        {
            "BlogId",
            "Url",
            "Rating"
        };
    }
}
