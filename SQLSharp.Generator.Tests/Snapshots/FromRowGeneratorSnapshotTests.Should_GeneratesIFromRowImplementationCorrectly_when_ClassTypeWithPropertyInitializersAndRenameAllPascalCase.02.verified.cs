﻿//HintName: ColumnAttribute.g.cs
// <auto-generated/>
#nullable enable

namespace SQLSharp.Generator.Result;

[global::System.AttributeUsage(validOn: global::System.AttributeTargets.Parameter | global::System.AttributeTargets.Property)]
public sealed class ColumnAttribute : global::System.Attribute
{
    public string Rename { get; set; } = string.Empty;
    public bool Flatten { get; set; } = false;
}