using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;

namespace LinqSql
{
    public static class ConnectionTestHelper
    {
        public const int CountCourses = 4;
        public const int CountStudents = 10;
        public const int CountCourseStudents = 2;

        public static DbConnection CreateConnection()
        {
            SQLiteConnection connection = new SQLiteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        }

        public static DbConnection CreatePopulatedConnection()
        {
            return CreateConnection()
                .PopulateTestTables()
                .PopulateTestData();
        }

        public static DbConnection PopulateTestTables(this DbConnection connection)
        {
            connection.CreateCourseTable();
            connection.CreateStudentTable();
            connection.CreateCourseStudentTable();
            return connection;
        }

        public static DbConnection PopulateTestData(this DbConnection connection)
        {
            Random random = new Random(DateTime.UtcNow.Millisecond);
            int[] courseIds = connection.PopulateCourseTable();
            int[] studentIds = connection.PopulateStudentTable();
            connection.PopulateCourseStudentTable(courseIds, studentIds, random);
            return connection;
        }

        private static void CreateCourseTable(this DbConnection connection)
        {
            connection.ExecuteNonQuery(
$@"create table Course (
    Id integer not null constraint PK_Course primary key autoincrement,
    Name varchar(255)
);");
        }

        private static void CreateStudentTable(this DbConnection connection)
        {
            connection.ExecuteNonQuery(
$@"create table Student (
    Id integer not null constraint PK_Student primary key autoincrement,
    FirstName varchar(255),
    LastName varchar(255)
);");
        }

        private static void CreateCourseStudentTable(this DbConnection connection)
        {
            connection.ExecuteNonQuery(
$@"create table CourseStudent (
    Id integer not null constraint PK_CourseStudent primary key autoincrement,
    CourseId,
    StudentId
);");
        }

        private static int[] PopulateCourseTable(this DbConnection connection)
        {
            LinkedList<int> ids = new LinkedList<int>();
            for (int i = 1; i <= CountCourses; i++)
            {
                connection.ExecuteNonQuery($"insert into Course (Name) values ('Course {i}')");
                ids.AddLast(connection.GetLastInsertId());
            }
            return ids.ToArray();
        }

        private static int[] PopulateStudentTable(this DbConnection connection)
        {
            LinkedList<int> ids = new LinkedList<int>();
            for (int i = 1; i <= CountStudents; i++)
            {
                connection.ExecuteNonQuery($"insert into Student (FirstName, LastName) values ('First Name {i}', 'Last Name {i}')");
                ids.AddLast(connection.GetLastInsertId());
            }
            return ids.ToArray();
        }

        private static void PopulateCourseStudentTable(this DbConnection connection, int[] courseIds, int[] studentIds, Random random)
        {
            foreach (int courseId in courseIds)
            {
                HashSet<int> available = new HashSet<int>(studentIds);
                for (int i = 0; i < CountCourseStudents && available.Any(); i++)
                {
                    int studentId = available.ElementAt(random.Next(available.Count));
                    available.Remove(studentId);
                    connection.ExecuteNonQuery($"insert into CourseStudent (CourseId, StudentId) values ({courseId}, {studentId})");
                }
            }
        }

        private static void ExecuteNonQuery(this DbConnection connection, string sql)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        private static int GetLastInsertId(this DbConnection connection)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "select last_insert_rowid()";
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
