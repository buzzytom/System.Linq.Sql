namespace System.Linq.Sql.Samples
{
    public static class BloggingContextSeeder
    {
        public static void Seed(this BloggingContext context)
        {
            context
                .Blogs
                .AddRange(
                    new Blog
                    {
                        Url = "http://blog1.com",
                        Rating = 5,
                        Posts =
                        {
                            new Post()
                            {
                                Content = "Post 1 content",
                                Title = "Post 1"
                            },
                            new Post()
                            {
                                Content = "Post 2 content",
                                Title = "Post 2"
                            },
                            new Post()
                            {
                                Content = "Post 3 content",
                                Title = "Post 3"
                            }
                        }
                    },
                    new Blog
                    {
                        Url = "http://blog2.com",
                        Rating = 7,
                        Posts =
                        {
                            new Post()
                            {
                                Content = "Post 4 content",
                                Title = "Post 4"
                            },
                            new Post()
                            {
                                Content = "Post 5 content",
                                Title = "Post 5"
                            },
                            new Post()
                            {
                                Content = "Post 6 content",
                                Title = "Post 6"
                            }
                        }
                    },
                    new Blog
                    {
                        Url = "http://blog3.com",
                        Rating = 4,
                        Posts =
                        {
                            new Post()
                            {
                                Content = "Post 7 content",
                                Title = "Post 7"
                            },
                            new Post()
                            {
                                Content = "Post 8 content",
                                Title = "Post 8"
                            },
                            new Post()
                            {
                                Content = "Post 9 content",
                                Title = "Post 9"
                            }
                        }
                    }
                );
            context.SaveChanges();
        }
    }
}
