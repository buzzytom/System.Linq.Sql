namespace System.Linq.Sql.Samples
{
    public class Post
    {
        public int PostId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int BlogId { get; set; }

        public Blog Blog { get; set; }

        public static string[] Fields { get; } =
        {
            "PostId",
            "Title",
            "Content",
            "BlogId"
        };
    }
}
