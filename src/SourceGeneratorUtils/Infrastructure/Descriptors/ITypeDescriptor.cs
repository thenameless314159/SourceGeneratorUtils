using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils;

/// <summary>
/// An equatable interface representing basic type identity.
/// </summary>
public interface ITypeDescriptor : IEquatable<ITypeDescriptor>
{
    /// <summary>
    /// Gets the type name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Fully qualified assembly name, prefixed with "global::", e.g. global::System.Numerics.BigInteger.
    /// </summary>
    string FullyQualifiedName { get; }

    /// <summary>
    /// Whether the type is a value type.
    /// </summary>
    bool IsValueType { get; }

    /// <summary>
    /// Gets the type kind.
    /// </summary>
    TypeKind TypeKind { get; }

    /// <summary>
    /// Gets the type special type.
    /// </summary>
    SpecialType SpecialType { get; }

    /// <summary>
    /// Whether the type can be null.
    /// </summary>
    bool CanBeNull { get; }
}