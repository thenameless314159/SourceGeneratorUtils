using static SourceGeneratorUtils.WellKnownStrings;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace SourceGeneratorUtils;

/// <summary>
/// An equatable value representing type identity.
/// </summary>
[DebuggerDisplay("Name={Name}")]
public sealed class TypeRef : ITypeDescriptor, IEquatable<TypeRef>
{
    /// <summary>
    /// Creates a new <see cref="TypeRef"/> from the given <see cref="ITypeSymbol"/>.
    /// </summary>
    /// <param name="type">The Roslyn type symbol.</param>
    public TypeRef(ITypeSymbol type)
    {
        Name = type.Name;
        TypeKind = type.TypeKind;
        IsValueType = type.IsValueType;
        SpecialType = type.SpecialType;
        FullyQualifiedName = type.GetFullyQualifiedAssemblyName();
    }

    /// <summary>
    /// Creates a new <see cref="TypeRef"/> from the given parameters.
    /// </summary>
    /// <param name="name">The name of the type.</param>
    /// <param name="namespace">The optional namespace of the type.</param>
    /// <param name="kind">The kind of the type.</param>
    /// <param name="isValueType">Whether the type is a value type.</param>
    /// <param name="specialType">The special type of the type.</param>
    public TypeRef(string name, string? @namespace, TypeKind kind, bool isValueType = false, SpecialType specialType = SpecialType.None)
        : this(!string.IsNullOrWhiteSpace(@namespace) ? $"{@namespace}.{name}" : name, kind, isValueType, specialType)
    {
    }

    /// <summary>
    /// Creates a new <see cref="TypeRef"/> from the given fully qualified type name and parameters.
    /// </summary>
    /// <param name="fullyQualifiedName">The fully qualified type name. May contains global alias.</param>
    /// <param name="kind">The kind of the type.</param>
    /// <param name="isValueType">Whether the type is a value type.</param>
    /// <param name="specialType">The special type of the type.</param>
    public TypeRef(string fullyQualifiedName, TypeKind kind, bool isValueType = false, SpecialType specialType = SpecialType.None)
    {
        TypeKind = kind;
        IsValueType = isValueType;
        SpecialType = specialType;

        // Extract the type name from the fully qualified name.
        ReadOnlySpan<char> fullyQualifiedNameSpan = fullyQualifiedName.AsSpan();
        int lastIndexOfDot = fullyQualifiedNameSpan.LastIndexOf('.');

        Name = lastIndexOfDot != -1
            ? WithoutGlobalAliasOrSelf(fullyQualifiedNameSpan[(lastIndexOfDot + 1)..].ToString())
            : WithoutGlobalAliasOrSelf(fullyQualifiedName);

        FullyQualifiedName = !fullyQualifiedName.StartsWith(GlobalAlias) 
            ? GlobalAlias + fullyQualifiedName
            : fullyQualifiedName;

        static string WithoutGlobalAliasOrSelf(string s) => s.StartsWith(GlobalAlias) ? s[GlobalAlias.Length..] : s;
    }

    /// <summary>
    /// Gets the type name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Fully qualified assembly name, prefixed with "global::", e.g. global::System.Numerics.BigInteger.
    /// </summary>
    public string FullyQualifiedName { get; }

    /// <summary>
    /// Whether the type is a value type.
    /// </summary>
    public bool IsValueType { get; }

    /// <summary>
    /// Gets the type kind.
    /// </summary>
    public TypeKind TypeKind { get; }

    /// <summary>
    /// Gets the type special type.
    /// </summary>
    public SpecialType SpecialType { get; }

    /// <summary>
    /// Whether the type can be null.
    /// </summary>
    public bool CanBeNull => !IsValueType || SpecialType is SpecialType.System_Nullable_T;

    /// <inheritdoc/>
    public bool Equals(TypeRef? other) => other != null && FullyQualifiedName == other.FullyQualifiedName;

    /// <inheritdoc/>
    public bool Equals(ITypeDescriptor? other) => other != null && FullyQualifiedName == other.FullyQualifiedName;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as TypeRef);

    /// <inheritdoc/>
    public override int GetHashCode() => FullyQualifiedName.GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => FullyQualifiedName;
}