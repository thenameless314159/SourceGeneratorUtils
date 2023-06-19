using static SourceGeneratorUtils.WellKnownStrings;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace SourceGeneratorUtils;

/// <summary>
/// An equatable value representing type identity with property or fields members.
/// </summary>
[DebuggerDisplay("Name={Name}")]
public sealed class TypeDesc : ITypeDescriptor, IEquatable<TypeDesc>
{
    private string? _cachedFullyQualifiedTypeName, _cachedFullyQualifiedAssemblyName;

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
    /// Gets or init whether the type is a record.
    /// </summary>
    public required bool IsRecord { get; init; }

    /// <summary>
    /// Gets or init the type modifier (sealed, unsafe, partial...).
    /// </summary>
    // review: may make an enum out of it
    public required string? TypeModifier { get; init; }

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
    public ImmutableEquatableArray<TypeDesc> Attributes { get; init; } = ImmutableEquatableArray<TypeDesc>.Empty;

    /// <summary>
    /// Gets or init the type interfaces.
    /// </summary>
    public ImmutableEquatableArray<TypeDesc> Interfaces { get; init; } = ImmutableEquatableArray<TypeDesc>.Empty;

    /// <summary>
    /// Gets or init the type base types.
    /// </summary>
    public ImmutableEquatableArray<TypeDesc> BaseTypes { get; init; } = ImmutableEquatableArray<TypeDesc>.Empty;

    /// <summary>
    /// Gets or init the type nested types.
    /// </summary>
    public ImmutableEquatableArray<TypeDesc> NestedTypes { get; init; } = ImmutableEquatableArray<TypeDesc>.Empty;

    /// <summary>
    /// Gets or init the type generic types.
    /// </summary>
    public ImmutableEquatableArray<TypeDesc> GenericTypes { get; init; } = ImmutableEquatableArray<TypeDesc>.Empty;

    /// <summary>
    /// Gets or init the type containing types.
    /// </summary>
    public ImmutableEquatableArray<TypeDesc> ContainingTypes { get; init; } = ImmutableEquatableArray<TypeDesc>.Empty;

    /// <summary>
    /// Gets or init the type members.
    /// </summary>
    public ImmutableEquatableArray<TypeMemberDesc> TypeMembers { get; init; } = ImmutableEquatableArray<TypeMemberDesc>.Empty;

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
        && IsRecord == other.IsRecord
        && TypeKind == other.TypeKind
        && TypeModifier == other.TypeModifier
        && Accessibility == other.Accessibility
        && Attributes.Equals(other.Attributes)
        && Interfaces.Equals(other.Interfaces)
        && BaseTypes.Equals(other.BaseTypes)
        && NestedTypes.Equals(other.NestedTypes)
        && GenericTypes.Equals(other.GenericTypes)
        && ContainingTypes.Equals(other.ContainingTypes)
        && TypeMembers.Equals(other.TypeMembers);

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
        TypeModifier,
        TypeKind,
        IsRecord,
        IsValueType,
        Accessibility,
        Attributes,
        Interfaces,
        BaseTypes,
        NestedTypes,
        GenericTypes,
        ContainingTypes,
        TypeMembers
    ).GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => FullyQualifiedName;
}