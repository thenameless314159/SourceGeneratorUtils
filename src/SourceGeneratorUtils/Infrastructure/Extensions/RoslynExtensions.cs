using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils;

/// <summary>
/// Provides extension methods to deal Roslyn types.
/// </summary>
public static class RoslynExtensions
{
    /// <summary>
    /// Gets the name of the given <paramref name="type"/> in a fully qualified style (including global alias).
    /// </summary>
    /// <param name="type">The type symbol.</param>
    /// <returns>The fully qualified type name.</returns>
    public static string GetFullyQualifiedAssemblyName(this ITypeSymbol type) 
        => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    /// <summary>
    /// Gets the <paramref name="accessibility"/> in a C# qualified style.
    /// </summary>
    /// <param name="accessibility">The accessibility.</param>
    /// <returns>The fully qualified accessibility.</returns>
    public static string GetAccessibilityString(this Accessibility accessibility) => accessibility switch
    {
        Accessibility.ProtectedAndInternal => "protected internal",
        Accessibility.ProtectedOrInternal => "public", // review: ??
        Accessibility.NotApplicable => string.Empty,
        Accessibility.Protected => "protected",
        Accessibility.Internal => "internal",
        Accessibility.Private => "private",
        Accessibility.Public => "public",
        _ => throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null)
    };

    /// <summary>
    /// Gets the <paramref name="typeKind"/> in a C# qualified style.
    /// </summary>
    /// <param name="typeKind">The type kind.</param>
    /// <param name="isRecord">Whether the type is a record.</param>
    /// <returns>The <paramref name="typeKind"/> in a fully qualified style.</returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static string GetTypeKindString(this TypeKind typeKind, bool isRecord = false) => typeKind switch
    {
        TypeKind.Class when isRecord => "record",
        TypeKind.Class when !isRecord => "class",
        TypeKind.Enum => "enum",
        TypeKind.Interface => "interface",
        TypeKind.Struct when isRecord => "record struct",
        TypeKind.Struct when !isRecord => "struct",
        _ => throw new ArgumentOutOfRangeException(nameof(typeKind), typeKind, null)
    };
}