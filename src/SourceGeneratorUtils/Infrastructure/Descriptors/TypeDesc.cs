using static SourceGeneratorUtils.WellKnownStrings;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace SourceGeneratorUtils;

/// <summary>
/// An equatable value representing basic type identity to be used for general-purpose source generation.
/// </summary>
[DebuggerDisplay("Name={Name}")]
public sealed class TypeDesc : ITypeDescriptor, IEquatable<TypeDesc>
{
    private string? _cachedFullyQualifiedTypeName, _cachedFullyQualifiedAssemblyName;

    /// <summary>
    /// Creates a new <see cref="TypeDesc"/> from the given parameters.
    /// </summary>
    /// <param name="name">The name of the type.</param>
    /// <param name="namespace">The optional namespace of the type.</param>
    /// <param name="typeKind">The kind of the type.</param>
    /// <param name="specialType">The special type.</param>
    /// <param name="accessibility">The type accessibility.</param>
    /// <param name="isValueType">Whether the type is a value type.</param>
    /// <param name="isReadOnly">Whether the type is readonly.</param>
    /// <param name="isAbstract">Whether the type is abstract.</param>
    /// <param name="isPartial">Whether the type is partial.</param>
    /// <param name="isRecord">Whether the type is a record.</param>
    /// <param name="isStatic">Whether the type is static.</param>
    /// <param name="isSealed">Whether the type is sealed.</param>
    /// <param name="attributes">The type attributes.</param>
    /// <param name="baseTypes">The type base types.</param>
    /// <param name="interfaces">The type interfaces.</param>
    /// <param name="genericTypes">The type generic types.</param>
    /// <param name="containingTypes">The type containing types.</param>
    /// <returns>A new <see cref="TypeDesc"/> with the given parameters.</returns>
    public static TypeDesc Create
    (
        string name,
        string? @namespace = null,
        TypeKind typeKind = TypeKind.Unknown,
        SpecialType specialType = SpecialType.None,
        Accessibility accessibility = Accessibility.NotApplicable,
        bool isValueType = false,
        bool isReadOnly = false,
        bool isAbstract = false,
        bool isPartial = false,
        bool isRecord = false,
        bool isStatic = false,
        bool isSealed = false,
        string[]? attributes = null,
        TypeDesc[]? baseTypes = null,
        TypeDesc[]? interfaces = null,
        TypeDesc[]? genericTypes = null,
        TypeDesc[]? containingTypes = null
    ) 
    => 
    new()
    {
        Name = name,
        Namespace = @namespace,
        TypeKind = typeKind,
        IsRecord = isRecord,
        IsStatic = isStatic,
        IsSealed = isSealed,
        IsPartial = isPartial,
        IsAbstract = isAbstract,
        IsReadOnly = isReadOnly,
        IsValueType = isValueType,
        SpecialType = specialType,
        Accessibility = accessibility,
        Attributes = ImmutableEquatableArray.Create(attributes ?? Array.Empty<string>()),
        BaseTypes = ImmutableEquatableArray.Create(baseTypes ?? Array.Empty<TypeDesc>()),
        Interfaces = ImmutableEquatableArray.Create(interfaces ?? Array.Empty<TypeDesc>()),
        GenericTypes = ImmutableEquatableArray.Create(genericTypes ?? Array.Empty<TypeDesc>()),
        ContainingTypes = ImmutableEquatableArray.Create(containingTypes ?? Array.Empty<TypeDesc>()),
        
    };

    /// <summary>
    /// Gets or init the type name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or init the namespace.
    /// </summary>
    public required string? Namespace { get; init; }

    /// <summary>
    /// Gets or init whether the type is a value type.
    /// </summary>
    public required bool IsValueType { get; init; }

    /// <summary>
    /// Gets or init whether the type has a readonly modifier.
    /// </summary>
    public required bool IsReadOnly { get; init; }

    /// <summary>
    /// Gets or init whether the type has an abstract modifier.
    /// </summary>
    public required bool IsAbstract { get; init; }

    /// <summary>
    /// Gets or init whether the type has a partial modifier.
    /// </summary>
    public required bool IsPartial { get; init; }
    
    /// <summary>
    /// Gets or init whether the type is a record.
    /// </summary>
    public required bool IsRecord { get; init; }

    /// <summary>
    /// Gets or init whether the type has a static modifier.
    /// </summary>
    public required bool IsStatic { get; init; }

    /// <summary>
    /// Gets or init whether the type has a sealed modifier.
    /// </summary>
    public required bool IsSealed { get; init; }

    /// <summary>
    /// Gets or init the type kind.
    /// </summary>
    public required TypeKind TypeKind { get; init; }

    /// <summary>
    /// Gets or init the special type.
    /// </summary>
    public required SpecialType SpecialType { get; init; }

    /// <summary>
    /// Gets or init the type accessibility.
    /// </summary>
    public required Accessibility Accessibility { get; init; }

    /// <summary>
    /// Gets or init the type attributes.
    /// </summary>
    /// <remarks>
    /// This is a string unlike other members because we want to allow for arbitrary attributes with parameters.
    /// </remarks>
    public ImmutableEquatableArray<string> Attributes { get; init; } = ImmutableEquatableArray<string>.Empty;

    /// <summary>
    /// Gets or init the type interfaces.
    /// </summary>
    public ImmutableEquatableArray<TypeDesc> Interfaces { get; init; } = ImmutableEquatableArray<TypeDesc>.Empty;

    /// <summary>
    /// Gets or init the type base types.
    /// </summary>
    public ImmutableEquatableArray<TypeDesc> BaseTypes { get; init; } = ImmutableEquatableArray<TypeDesc>.Empty;

    /// <summary>
    /// Gets or init the type generic types.
    /// </summary>
    public ImmutableEquatableArray<TypeDesc> GenericTypes { get; init; } = ImmutableEquatableArray<TypeDesc>.Empty;

    /// <summary>
    /// Gets or init the type containing types.
    /// </summary>
    public ImmutableEquatableArray<TypeDesc> ContainingTypes { get; init; } = ImmutableEquatableArray<TypeDesc>.Empty;

    /// <inheritdoc/>
    public string FullyQualifiedName => FullyQualifiedAssemblyName;

    /// <summary>
    /// Fully qualified type name, prefixed with its namespace, e.g. System.Numerics.BigInteger.
    /// </summary>
    public string FullyQualifiedTypeName => _cachedFullyQualifiedTypeName 
        ??= !string.IsNullOrWhiteSpace(Namespace) ? Namespace + "." + Name : Name;

    /// <summary>
    /// Fully qualified assembly name, prefixed with "global::", e.g. global::System.Numerics.BigInteger.
    /// </summary>
    public string FullyQualifiedAssemblyName => _cachedFullyQualifiedAssemblyName ??= GlobalAlias + FullyQualifiedTypeName;

    /// <summary>
    /// Whether the type can be null.
    /// </summary>
    public bool CanBeNull => !IsValueType || SpecialType is SpecialType.System_Nullable_T;

    /// <inheritdoc/>
    public bool Equals(TypeDesc? other) => other != null 
        && Name == other.Name
        && Namespace == other.Namespace
        && IsValueType == other.IsValueType
        && IsReadOnly == other.IsReadOnly
        && IsAbstract == other.IsAbstract
        && IsPartial == other.IsPartial
        && IsStatic == other.IsStatic
        && IsSealed == other.IsSealed
        && IsRecord == other.IsRecord
        && TypeKind == other.TypeKind
        && Accessibility == other.Accessibility
        && Attributes.Equals(other.Attributes)
        && Interfaces.Equals(other.Interfaces)
        && BaseTypes.Equals(other.BaseTypes)
        && GenericTypes.Equals(other.GenericTypes)
        && ContainingTypes.Equals(other.ContainingTypes);

    /// <inheritdoc/>
    public bool Equals(ITypeDescriptor? other) => (other is TypeDesc typeDesc && Equals(typeDesc))
        || other is not null && FullyQualifiedName == other.FullyQualifiedName;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as TypeDesc);

    /// <inheritdoc/>
    // review: Currently relies on ValueTuple comparison for convenience and
    // due to lack of HashCode.Combine() in netstandard2.0, may implement it manually in the future.
    public override int GetHashCode() =>
    ( 
        Name,
        Namespace,
        IsValueType,
        IsReadOnly,
        IsAbstract,
        IsPartial,
        IsStatic,
        IsSealed,
        IsRecord,
        TypeKind,
        IsRecord,
        IsValueType,
        Accessibility,
        Attributes,
        Interfaces,
        BaseTypes,
        GenericTypes,
        ContainingTypes
    ).GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => FullyQualifiedName;
}