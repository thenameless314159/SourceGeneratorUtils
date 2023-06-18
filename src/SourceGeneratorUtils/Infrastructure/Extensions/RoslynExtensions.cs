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
}