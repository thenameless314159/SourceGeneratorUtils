using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SourceGeneratorUtils;

/// <summary>
/// Provides extension methods to deal Roslyn types.
/// </summary>
public static partial class RoslynExtensions
{
    /// <summary>
    /// Returns the <see cref="LanguageVersion"/> for the given <paramref name="compilation"/> if it is assignable to <see cref="CSharpCompilation"/>.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The language version.</returns>
    public static LanguageVersion? GetLanguageVersion(this Compilation compilation)
        => compilation is CSharpCompilation csc ? csc.LanguageVersion : null;

    /// <summary>
    /// Gets the name of the given <paramref name="type"/> in a fully qualified style (including global alias).
    /// </summary>
    /// <param name="type">The type symbol.</param>
    /// <returns>The fully qualified type name.</returns>
    public static string GetFullyQualifiedAssemblyName(this ITypeSymbol type) 
        => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    /// <summary>
    /// Creates a copy of the Location instance that does not capture a reference to Compilation.
    /// </summary>
    [return: NotNullIfNotNull(nameof(location))]
    public static Location? GetTrimmedLocation(this Location? location)
        => location is null ? null : Location.Create(location.SourceTree?.FilePath ?? "", location.SourceSpan, location.GetLineSpan().Span);

    /// <summary>
    /// Gets the <paramref name="accessibility"/> in a C# qualified style.
    /// </summary>
    /// <param name="accessibility">The accessibility.</param>
    /// <returns>The fully qualified accessibility.</returns>
    public static string GetAccessibilityString(this Accessibility accessibility) => accessibility switch
    {
        Accessibility.ProtectedAndInternal => "protected internal",
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
        TypeKind.Struct or TypeKind.Structure when isRecord => "record struct",
        TypeKind.Struct or TypeKind.Structure when !isRecord => "struct",
        _ => throw new ArgumentOutOfRangeException(nameof(typeKind), typeKind, null)
    };
}