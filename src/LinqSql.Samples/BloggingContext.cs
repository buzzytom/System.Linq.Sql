using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Microsoft.EntityFrameworkCore;

using System.Linq.Sql.Sqlite;

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

        public IQueryable<Record> Query(string table, IEnumerable<string> fields)
        {
            return new SqliteQueryable(Database.GetDbConnection(), table, fields);
        }

        public IQueryable<Record> Query(string table, string alias, IEnumerable<string> fields)
        {
            return new SqliteQueryable(Database.GetDbConnection(), table, alias, fields);
        }

        // ----- Properties ----- //

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
}
