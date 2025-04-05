## SQL-Sharp

C# Library for ADO.net providing a middle ground between Dapper and Rust's sqlx library.

### Features
1. Extension methods for IDbConnection and DbConnection to execute queries that deserialize to an 
`IFromRow` implementation or a scalar value.
2. Source generate `IFromRow` implementations for classes and structs.
   1. Only works for definitions that contains a constructor
      1. For structs, largest constructor is used
      2. For classes, smallest constructor is used
   2. Allows renaming single or all fields when parsing result set
   3. Allows flattening a result into nested objects (but not nested arrays)
   4. Allows handling of fields whose type is `IDbDecode`

### TODO
* Add property initializer ability for definition without constructors
* Add ability to construct parameter/property with user defined expression