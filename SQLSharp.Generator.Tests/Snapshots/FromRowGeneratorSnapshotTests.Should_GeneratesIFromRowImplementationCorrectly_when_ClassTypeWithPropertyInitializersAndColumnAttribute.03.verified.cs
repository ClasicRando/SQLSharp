﻿//HintName: RowParsers.GeneratedRow.g.cs
// <auto-generated/>
#nullable enable

partial class GeneratedRow : SQLSharp.Result.IFromRow<GeneratedRow>
{
    public static GeneratedRow FromRow(SQLSharp.Result.IDataRow row)
    {
        return new GeneratedRow
        {
            Id = row.GetFieldNotNull<System.Guid>("Id"),
            Name = row.GetFieldNotNull<System.String>("Name"),
            Age = row.GetFieldNotNull<System.Byte>("Age"),
            DateOfBirth = row.GetField<System.DateTime?>("DateOfBirth"),
        };
    }
}