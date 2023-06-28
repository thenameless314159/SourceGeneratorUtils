using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.SourceGeneration;

internal sealed class KnownTypeSymbols
{
    public KnownTypeSymbols(Compilation compilation) => Compilation = compilation;

    public Compilation Compilation { get; }

    public INamedTypeSymbol? IndexType => GetOrResolveType(typeof(Index), ref _indexType);
    private Option<INamedTypeSymbol?> _indexType;

    public INamedTypeSymbol? RangeType => GetOrResolveType(typeof(Range), ref _rangeType);
    private Option<INamedTypeSymbol?> _rangeType;

    public INamedTypeSymbol? IsExternalInitType => GetOrResolveType(typeof(IsExternalInit), ref _isExternalInitType);
    private Option<INamedTypeSymbol?> _isExternalInitType;

    public INamedTypeSymbol? NotNullWhenAttributeType => GetOrResolveType(typeof(NotNullWhenAttribute), ref _notNullWhenAttributeType);
    private Option<INamedTypeSymbol?> _notNullWhenAttributeType;

    public INamedTypeSymbol? DoesNotReturnAttributeType => GetOrResolveType(typeof(DoesNotReturnAttribute), ref _doesNotReturnAttributeType);
    private Option<INamedTypeSymbol?> _doesNotReturnAttributeType;

    public INamedTypeSymbol? NotNullIfNotNullAttributeType => GetOrResolveType(typeof(NotNullIfNotNullAttribute), ref _notNullIfNotNullAttributeType);
    private Option<INamedTypeSymbol?> _notNullIfNotNullAttributeType;

    public INamedTypeSymbol? RequiredMemberAttributeType => GetOrResolveType(typeof(RequiredMemberAttribute), ref _requiredMemberAttributeType);
    private Option<INamedTypeSymbol?> _requiredMemberAttributeType;

    public INamedTypeSymbol? SetsRequiredMembersAttributeType => GetOrResolveType(typeof(SetsRequiredMembersAttribute), ref _setsRequiredMembersAttributeType);
    private Option<INamedTypeSymbol?> _setsRequiredMembersAttributeType;

    public INamedTypeSymbol? CompilerFeatureRequiredAttributeType => GetOrResolveType(typeof(CompilerFeatureRequiredAttribute), ref _compilerFeatureRequiredAttributeType);
    private Option<INamedTypeSymbol?> _compilerFeatureRequiredAttributeType;

    /// <summary>
    /// Whether the specified <paramref name="resourceFileName"/> polyfill is defined in this instance <see cref="Compilation"/>.
    /// </summary>
    /// <param name="resourceFileName">The resource file name computed in <see cref="EmbeddedResourcesStore.FileNamesByResourceName"/>.</param>
    /// <returns>Whether the specified polyfill is defined in this instance <see cref="Compilation"/>.</returns>
    public bool IsPolyfillDefined(string resourceFileName) => resourceFileName switch
    {
        "System.Index.g.cs" => IndexType is not null,
        "System.Range.g.cs" => RangeType is not null,
        "System.Runtime.CompilerServices.IsExternalInit.g.cs" => IsExternalInitType is not null,
        "System.Diagnostics.CodeAnalysis.NotNullWhenAttribute.g.cs" => NotNullWhenAttributeType is not null,
        "System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute.g.cs" => DoesNotReturnAttributeType is not null,
        "System.Runtime.CompilerServices.RequiredMemberAttribute.g.cs" => RequiredMemberAttributeType is not null,
        "System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute.g.cs" => NotNullIfNotNullAttributeType is not null,
        "System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute.g.cs" => SetsRequiredMembersAttributeType is not null,
        "System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute.g.cs" => CompilerFeatureRequiredAttributeType is not null,
        _ => false
    };

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
    