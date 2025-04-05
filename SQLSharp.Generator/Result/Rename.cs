using System.Text;
using System.Text.RegularExpressions;

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

public static class RenameExtensions
{
    private static readonly Regex IsNotAlphaNumeric = new("[^0-9a-zA-Z]", RegexOptions.Compiled);
    
    public static Rename ParseToRename(this object value)
    {
        if (value is int i)
        {
            return (Rename)i;
        }
        return Rename.None;
    }

    public static string TransformRowFieldName(this Rename rename, string rowFieldName)
    {
        return rename switch
        {
            Rename.SnakeCase => Transform(
                rowFieldName,
                Lowercase,
                builder => builder.Append('_')),
            Rename.CamelCase => ToCamelCase(rowFieldName),
            Rename.PascalCase => Transform(
                rowFieldName,
                Capitalize,
                _ => { }),
            Rename.UpperCase => rowFieldName.ToUpperInvariant(),
            Rename.LowerCase => rowFieldName.ToLowerInvariant(),
            Rename.None => rowFieldName,
            _ => rowFieldName,
        };
    }

    private static string ToCamelCase(string s)
    {
        var first = true;
        return Transform(
            s,
            (str, builder) =>
            {
                if (first)
                {
                    first = false;
                    Lowercase(str, builder);
                }
                else
                {
                    Capitalize(str, builder);
                }
            },
            _ => { });
    }

    private static void Lowercase(string s, StringBuilder builder)
    {
        foreach (var c in s)
        {
            builder.Append(char.ToLowerInvariant(c));
        }
    }

    private static void Capitalize(string s, StringBuilder builder)
    {
        if (string.IsNullOrEmpty(s))
        {
            return;
        }

        builder.Append(char.ToUpperInvariant(s[0]));
        if (s.Length <= 1) return;
        
        for (var i = 1; i < s.Length; ++i)
        {
            builder.Append(s[i]);
        }
    }

    private enum WordMode
    {
        Boundary,
        Lowercase,
        Uppercase,
    }

    private static string Transform(
        string s,
        Action<string, StringBuilder> withWord,
        Action<StringBuilder> boundary)
    {
        var builder = new StringBuilder();
        var firstWord = true;
        
        foreach (var word in IsNotAlphaNumeric.Split(s))
        {
            var init = 0;
            var mode = WordMode.Boundary;

            for (var i = 0; i < word.Length; i++)
            {
                var currentChar = word[i];
                if (i < word.Length - 1)
                {
                    var nextChar = word[i + 1];
                    WordMode nextMode;
                    if (char.IsLower(currentChar))
                    {
                        nextMode = WordMode.Lowercase;
                    }
                    else if (char.IsUpper(currentChar))
                    {
                        nextMode = WordMode.Uppercase;
                    }
                    else
                    {
                        nextMode = mode;
                    }

                    if (nextMode == WordMode.Lowercase && char.IsUpper(nextChar))
                    {
                        if (!firstWord)
                        {
                            boundary(builder);
                        }

                        withWord(word.Substring(init, i - init + 1), builder);
                        firstWord = false;
                        init = i + 1;
                        mode = WordMode.Boundary;
                    }
                    else if (mode == WordMode.Uppercase && char.IsUpper(currentChar) &&
                             char.IsLower(nextChar))
                    {
                        if (!firstWord)
                        {
                            boundary(builder);
                        }
                        else
                        {
                            firstWord = false;
                        }

                        withWord(word.Substring(init, i - init + 1), builder);
                        init = i;
                        mode = WordMode.Boundary;
                    }
                    else
                    {
                        mode = nextMode;
                    }
                }
                else
                {
                    if (!firstWord)
                    {
                        boundary(builder);
                    }
                    else
                    {
                        firstWord = false;
                    }
                    withWord(word.Substring(init), builder);
                    break;
                }
            }
        }

        return builder.ToString();
    }
}
