﻿//HintName: RowParsers.GeneratedRow.g.cs
// <auto-generated/>
#nullable enable

using SQLSharp.Result;

partial class GeneratedRow : IFromRow<GeneratedRow>
{
    public static GeneratedRow FromRow(IDataRow row)
    {
        return new GeneratedRow(
            original: row.GetFieldNotNull<GeneratedRow>("original"));
    }
}