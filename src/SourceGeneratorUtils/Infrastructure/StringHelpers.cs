namespace SourceGeneratorUtils;

public static class StringHelpers
{
    /// <summary>
    /// Returns a new string with the first character in upper case.
    /// </summary>
    /// <param name="str">The input <see cref="string"/>.</param>
    /// <returns>A new string with the first character in upper case</returns>
    /// <exception cref="ArgumentNullException"><see pref="str"/> is null or whitespace.</exception>
    public static string FirstCharToUpperInvariant(this string str)
    {
        if (string.IsNullOrWhiteSpace(str)) 
            throw new ArgumentNullException(nameof(str));

        if (char.IsUpper(str[0])) 
            return str;

        var array = str.ToCharArray();
        array[0] = char.ToUpperInvariant(array[0]);

        return new string(array);
    }

    /// <summary>
    /// Returns a new string with the first character in lower case.
    /// </summary>
    /// <param name="str">The input <see cref="string"/>.</param>
    /// <returns>A new string with the first character in lower case</returns>
    /// <exception cref="ArgumentNullException"><see pref="str"/> is null or whitespace.</exception>
    public static string FirstCharToLowerInvariant(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            throw new ArgumentNullException(nameof(str));

        if (char.IsLower(str[0]))
            return str;

        var array = str.ToCharArray();
        array[0] = char.ToLowerInvariant(array[0]);

        return new string(array);
    }

    /// <summary>
    /// Returns the string with an '@' prefix if it is a C# keyword.
    /// </summary>
    /// <param name="str">The input <see cref="string"/>.</param>
    /// <returns>Self if not a c# keyword, otherwise the keyword prefixed by an '@'.</returns>
    public static string OverridenCSharpKeywordOrSelf(string str) => str switch
    {
        "abstract" => "@abstract",
        "as" => "@as",
        "base" => "@base",
        "bool" => "@bool",
        "break" => "@break",
        "byte" => "@byte",
        "case" => "@case",
        "catch" => "@catch",
        "char" => "@char",
        "checked" => "@checked",
        "class" => "@class",
        "const" => "@const",
        "continue" => "@continue",
        "decimal" => "@decimal",
        "default" => "@default",
        "delegate" => "@delegate",
        "do" => "@do",
        "double" => "@double",
        "else" => "@else",
        "enum" => "@enum",
        "event" => "@event",
        "explicit" => "@explicit",
        "extern" => "@extern",
        "false" => "@false",
        "finally" => "@finally",
        "fixed" => "@fixed",
        "float" => "@float",
        "for" => "@for",
        "foreach" => "@foreach",
        "goto" => "@goto",
        "if" => "@if",
        "implicit" => "@implicit",
        "in" => "@in",
        "int" => "@int",
        "interface" => "@interface",
        "internal" => "@internal",
        "is" => "@is",
        "lock" => "@lock",
        "long" => "@long",
        "namespace" => "@namespace",
        "new" => "@new",
        "null" => "@null",
        "object" => "@object",
        "operator" => "@operator",
        "out" => "@out",
        "override" => "@override",
        "params" => "@params",
        "private" => "@private",
        "protected" => "@protected",
        "public" => "@public",
        "readonly" => "@readonly",
        "ref" => "@ref",
        "return" => "@return",
        "sbyte" => "@sbyte",
        "sealed" => "@sealed",
        "short" => "@short",
        "sizeof" => "@sizeof",
        "stackalloc" => "@stackalloc",
        "static" => "@static",
        "string" => "@string",
        "struct" => "@struct",
        "switch" => "@switch",
        "this" => "@this",
        "throw" => "@throw",
        "true" => "@true",
        "try" => "@try",
        "typeof" => "@typeof",
        "uint" => "@uint",
        "ulong" => "@ulong",
        "unchecked" => "@unchecked",
        "unsafe" => "@unsafe",
        "ushort" => "@ushort",
        "using" => "@using",
        "virtual" => "@virtual",
        "void" => "@void",
        "volatile" => "@volatile",
        "while" => "@while",
        _ => str
    };
}