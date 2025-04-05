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
            id: row.GetFieldNotNull<Guid>("ID"),
            name: row.GetFieldNotNull<String>("NAME"),
            age: row.GetFieldNotNull<Byte>("AGE"),
            dateOfBirth: row.GetField<DateTime?>("DATEOFBIRTH"));
    }
}