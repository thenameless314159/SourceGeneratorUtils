using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.SourceGeneration;

internal sealed class KnownTypeSymbols
{
    public KnownTypeSymbols(Compilation compilation) => Compilation = compilation;

    public Compilation Compilation { get; }

    private INamedTypeSymbol? GetOrResolveType(Type type, ref Option<INamedTypeSymbol?> field)
        => GetOrResolveType(type.FullName!, ref field);

    private INamedTypeSymbol? GetOrResolveType(string fullyQualifiedName, ref Option<INamedTypeSymbol?> field)
    {
        if (field.HasValue)
        {
            return field.Value;
        }

        INamedTypeSymbol? type = Compilation.GetBestTypeByMetadataName(fullyQualifiedName);
        field = new(type);
        return type;
    }

    private readonly struct Option<T>
    {
        public readonly bool HasValue;
        public readonly T Value;

        public Option(T value)
        {
            HasValue = true;
            Value = value;
        }
    }
}
    