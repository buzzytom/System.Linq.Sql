System.Linq.Sql
===============

*System.Linq.Sql* is a light weight set of extensions for *System.Linq* for
making database operations on types that are not known at compile time.

Purpose
-------

You wouldn’t be alone if you didn’t understand the short description, so
consider a simple use case: what if we wanted to allow users to define custom
fields or even tables in a system. How would we define this at compile time? You
could define a general purpose table which defines a “table”, “field” and
“value” column, which would work with frameworks like Entity Framework, but let
us all agree that this is messy.

Solution, query on tables created at run-time, enter System.Linq.Sql.

Getting Started
---------------

### Installation

Install the following package:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
System.Linq.Sql
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

otherwise, clone the master branch of this repository and reference the
*System.Linq.Sql* project as well any database vendor specific package you
require.

### Constructing Queries

Use a `Linq` like syntax to build queries, this is best shown with an example:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ c#
IQueryable<Record> query = new SqliteQueryable(connection, "Blogs", fields);

// Traditional linq syntax can be used to predicate your queries
// When comparing a field value you must specify the table and field with this square bracket style:
// ["table"]["field"]
query = query.Where(x => (int)x["Blogs"]["BlogId"] == 1 || (int)x["Blogs"]["BlogId"] == 2);

// Executing the query can be achieved with methods like ToArray, ToList, FirstOrDefault etc.
// Note: Helper methods exist to flatten results which we will cover in other samples
Record[] results = query.ToArray();
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

### Samples

There is a sample project, you should clone the repository and run it. It has
samples of most the queries you would need to run.

[Samples
Directory](https://github.com/buzzytom/System.Linq.Sql/tree/master/src/LinqSql.Samples)

Requirements
------------

### Dependencies

This project was built with C\# and .Net Standard 2.0 and as such is compatible
with everything it would normally be compatible with.

### Supported Database Engines

System.Linq.Sql is packaged with a default SQL translator which is compatible
with Microsoft SQL Server. For specific database engines a different namespace
is used (as well as NuGet package). The following table shows a list of database
engines and the namespace which should be used:

| Database Engine | Dependency             | Namespace              |
|-----------------|------------------------|------------------------|
| ISO/IEC 9075    | System.Linq.Sql        | System.Linq.Sql        |
| SQL Server      | System.Linq.Sql        | System.Linq.Sql        |
| SQLite          | System.Linq.Sql.Sqlite | System.Linq.Sql.Sqlite |
| PostgreSQL      | **planned**            | **planned**            |

If other database engine support is needed, please check if an issue has not
already been raised otherwise create one. If one has been created, like or make
a “me too” comment on the issue.

### Design Requirements

Because tables and fields are only known at run-time, we recommend you have a
way of indexing your tables and fields. We recommend having a separate mechanism
for describing the meta structure of your system. This can be as simple as
declaring a `FieldDefinition` table. E.g:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ c#
public class FieldDefinition
{
    public FieldDefinition()
    { }

    public int Id { set; get; }

    public string TableName { set; get; }

    public string FieldName { set; get; }

    /// Optional, e.g: database: VARCHAR, BIGINT etc...
    public string Type { set; get; }
}
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Road Map
--------

These are planned future features:

### 1.1.0

-   Update

-   Insert

-   Delete

### 1.2.0

-   PostgreSQL support

### 1.3.0

-   Implicit table names (this would allow `x => x[”FieldName”] == “Something”`
    without the table name)

-   Optional field type checking, this would allow run-time checks of query
    parameters types if the field definitions type is specified

### 1.4.0

-   Group By

License
-------

See LICENSE.md.
