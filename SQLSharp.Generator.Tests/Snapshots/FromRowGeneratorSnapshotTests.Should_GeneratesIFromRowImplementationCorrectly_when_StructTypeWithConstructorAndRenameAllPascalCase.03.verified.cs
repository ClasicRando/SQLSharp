﻿//HintName: RowParsers.GeneratedRow.g.cs
// <auto-generated/>
#nullable enable

using SQLSharp.Result;
using System;

partial struct GeneratedRow : IFromRow<GeneratedRow>
{
    public static GeneratedRow FromRow(IDataRow row)
    {
        return new GeneratedRow(
            id: row.GetFieldNotNull<Guid>("Id"),
            name: row.GetFieldNotNull<String>("Name"),
            age: row.GetFieldNotNull<Byte>("Age"),
            dateOfBirth: row.GetField<DateTime?>("DateOfBirth"));
    }
}