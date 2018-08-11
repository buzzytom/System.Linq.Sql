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

`public class FieldDefinition`

`{`

`public FieldDefinition()`

`{ }`

 

`public int Id { set; get; }`

\` \`

`public string TableName { set; get; }`

 

`public string FieldName { set; get; }`

\` \`

`/// Optional, e.g: database: VARCHAR, BIGINT etc...`

`public string Type { set; get; }`

`}`

Road Map
--------

These are planned future features:

### 1.1.0

-   Update

-   Insert

-   Delete

### 1.2.0

-   PostgreSQL support

-   Oracle SQL support

License
-------

See LICENSE.md.
