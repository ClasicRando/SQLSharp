## SQL-Sharp

C# Library for ADO.net providing a middle ground between Dapper and Rust's sqlx library.

### Features
#### Extension Methods
Extension methods for IDbConnection and DbConnection to execute queries that deserialize to an
`IFromRow` implementation or a scalar value.

### Source Generation
Source generation is available for `IFromRow` implementations for classes and structs using the
`[FromRow]` attribute. __Note__: Definition must be partial to allow extending definition with
`IFromRow` interface. The preferred method to data model definition is using a constructor. If your
definition contains a non-default constructor then that will be used in source generation with
different behaviour for structs and classes:

* For structs, largest constructor is used 
* For classes, smallest constructor is used

To support all instance creation patterns, you can also initialize your instances through init/set
properties if a no-arg constructor is the only option. In this case, all non-readonly properties are
initialized during an instance creation statement;

Along with source generation of instance creation, you can customize how fields are mapped to row
fields by using the `[Column]` attribute. The 2 options are:

1. `[Column(Rename = "custom_field_name")]`
   1. Allows for specifying a field name that isn't the parameter/property name
2. `[Column(Flatten = true)]`
   1. When specified, the property is treated as a nested object within the result row and allows
   for parsing using a `IFromRow` implementation

### TODO
* Postgresql composite type support
* Support for mapping enum types
* SQLServer table value type support
* SQLServer UDF type support
* Bulk copy helpers?