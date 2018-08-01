using System.Data;
using System.Data.Common;

using Microsoft.EntityFrameworkCore;

namespace System.Linq.Sql.Samples
{
    public class BloggingContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=:memory:");
        }

        public DbConnection GetDbConnection()
        {
            DbConnection connection = Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return connection;
        }

        // ----- Properties ----- //

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
}
