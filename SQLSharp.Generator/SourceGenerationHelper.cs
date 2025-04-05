﻿using System.Text;
using Microsoft.CodeAnalysis;
using SQLSharp.Generator.Result;

namespace SQLSharp.Generator;

public static class SourceGenerationHelper
{
    private static readonly DiagnosticDescriptor TypeIsNotPartial =
        new(
            "SQLSHARP001",
            "Annotated row type is not partial",
            "'{0}' must be partial to allow for adding extra behaviour",
            "FromRowGenerator",
            DiagnosticSeverity.Error,
            true);

    private static readonly DiagnosticDescriptor MissingPrimaryConstructor =
        new(
            "SQLSHARP002",
            "Could not find a primary constructor",
            "'{0}' does not have a primary constructor for row parsing",
            "FromRowGenerator",
            DiagnosticSeverity.Error,
            true);

    public const string RenameEnum =
        """
        // <auto-generated/>
        #nullable enable

        namespace SQLSharp.Generator.Result;
        
        public enum Rename
        {
            SnakeCase,
            CamelCase,
            PascalCase,
            UpperCase,
            LowerCase,
            None,
        }
        """;

    public const string FromRowAttribute =
        """
        // <auto-generated/>
        #nullable enable

        namespace SQLSharp.Generator.Result;

        [global::System.AttributeUsage(validOn: global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct)]
        public sealed class FromRowAttribute : global::System.Attribute
        {
            public Rename RenameAll { get; set; } = Rename.None;
        }
        """;

    public const string ColumnAttribute =
        """
        // <auto-generated/>
        #nullable enable

        namespace SQLSharp.Generator.Result;

        [global::System.AttributeUsage(validOn: global::System.AttributeTargets.Parameter | global::System.AttributeTargets.Property)]
        public sealed class ColumnAttribute : global::System.Attribute
        {
            public string Rename { get; set; } = string.Empty;
            public bool Flatten { get; set; } = false;
        }
        """;

    public static string GenerateRowParserPartialClass(
        RowParserToGenerate rowParserToGenerate,
        SourceProductionContext sourceProductionContext)
    {
        if (!rowParserToGenerate.IsPartial)
        {
            sourceProductionContext.ReportDiagnostic(
                Diagnostic.Create(TypeIsNotPartial, Location.None,
                    rowParserToGenerate.Name));
            return string.Empty;
        }

        List<string> parameterNamespaces = [];
        ConstructorData? constructor;
        if (rowParserToGenerate.IsStruct)
        {
            constructor = rowParserToGenerate.Constructors
                .OrderByDescending(c => c.Parameters.Length)
                .FirstOrDefault();
        }
        else
        {
            constructor = rowParserToGenerate.Constructors
                .OrderBy(c => c.Parameters.Length)
                .FirstOrDefault();
        }

        if (constructor is null)
        {
            sourceProductionContext.ReportDiagnostic(
                Diagnostic.Create(MissingPrimaryConstructor, Location.None,
                    rowParserToGenerate.Name));
            return string.Empty;
        }

        var builder = new StringBuilder(constructor.Parameters.Length > 0
            ? "\n            "
            : string.Empty);
        for (var index = 0; index < constructor.Parameters.Length; index++)
        {
            ParameterData parameter = constructor.Parameters[index];
            if (!string.IsNullOrEmpty(parameter.TypeData.ContainingNamespace))
            {
                parameterNamespaces.Add(parameter.TypeData.ContainingNamespace);
            }

            builder.Append(parameter.Name);
            builder.Append(": ");
            
            var typeName = parameter.TypeData.Name;

            if (parameter.Flatten)
            {
                builder.Append(typeName);
                builder.Append(".FromRow(row)");
                continue;
            }


            builder.Append("row.GetField");
            if (!parameter.TypeData.IsNullable)
            {
                builder.Append("NotNull");
            }

            builder.Append('<');
            builder.Append(typeName);
            if (parameter.TypeData is { IsRefType: false, IsNullable: true })
            {
                builder.Append('?');
            }
            builder.Append('>');
            builder.Append("(\"");
            var fieldName = parameter.HasRename
                ? parameter.ResultFieldName
                : rowParserToGenerate.Rename.TransformRowFieldName(parameter.ResultFieldName);
            builder.Append(fieldName);
            builder.Append("\")");

            if (index >= constructor.Parameters.Length - 1) continue;

            builder.Append(',');
            builder.AppendLine();
            builder.Append("            ");
        }

        var namespaces = parameterNamespaces.Count == 0
            ? ""
            : "\n" + string.Join(
                "\n",
                parameterNamespaces.Distinct().Select(n => $"using {n};"));
        var type = rowParserToGenerate.IsStruct ? "struct" : "class";
        var typeNamespace = string.IsNullOrWhiteSpace(rowParserToGenerate.Namespace)
            ? string.Empty
            : $"\nnamespace {rowParserToGenerate.Namespace};\n";
        return
            $$"""
              // <auto-generated/>
              #nullable enable
              
              using SQLSharp.Result;{{namespaces}}
              {{typeNamespace}}
              partial {{type}} {{rowParserToGenerate.Name}} : IFromRow<{{rowParserToGenerate.Name}}>
              {
                  public static {{rowParserToGenerate.Name}} FromRow(IDataRow row)
                  {
                      return new {{rowParserToGenerate.Name}}({{builder}});
                  }
              }
              """;
    }
}