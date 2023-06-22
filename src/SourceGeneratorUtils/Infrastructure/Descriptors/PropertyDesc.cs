using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils;

/// <summary>
/// Models common specifications for a type member (property or field).
/// </summary>
public sealed record PropertyDesc : ITypeDescriptor
{
    /// <summary>
    /// The exact name specified in the source code. This might be different
    /// from the <see cref="MemberName"/> because source code might be decorated
    /// with '@' for reserved keywords, e.g. public string @event { get; set; }
    /// </summary>
    public string? NameSpecifiedInSourceCode { get; init; }

    /// <summary>
    /// Gets or init the name of the member.
    /// </summary>
    public required string MemberName { get; init; }

    /// <summary>
    /// Gets or init the optional name of the var for the member.
    /// </summary>
    public string? MemberVarName { get; init; }

    /// <summary>
    /// Gets or init a reference to the member type.
    /// </summary>
    public required TypeRef MemberType { get; init; }

    /// <summary>
    /// Gets or init a reference to the declaring type of the property.
    /// </summary>
    /// <remarks>
    /// This have been made a <see cref="ITypeDescriptor"/> because it could either be a <see cref="TypeRef"/> or a <see cref="TypeDesc"/>.
    /// </remarks>
    public required ITypeDescriptor DeclaringType { get; init; }

    /// <summary>
    /// Gets or init the accessibility of the member.
    /// </summary>
    public required Accessibility MemberAccessibility { get; init; }

    /// <summary>
    /// Gets or init the accessibility of the getter. This is only applicable to properties
    /// and will be set to <see cref="Accessibility.NotApplicable"/> for fields.
    /// </summary>
    public required Accessibility GetterAccessibility { get; init; }

    /// <summary>
    /// Gets or init the accessibility of the setter. This is only applicable to properties
    /// and will be set to <see cref="Accessibility.NotApplicable"/> for fields.
    /// </summary>
    public required Accessibility SetterAccessibility { get; init; }

    /// <summary>
    /// Gets or init the attributes of the member.
    /// </summary>
    public ImmutableEquatableArray<string> Attributes { get; init; } = ImmutableEquatableArray<string>.Empty;

    /// <summary>
    /// The optional order of the property if any.
    /// </summary>
    public int? Order { get; init; }

    /// <summary>
    /// If representing a property, should return true if either the getter or setter are public.
    /// Otherwise whether the field has a readonly modifier.
    /// </summary>
    public required bool IsReadOnly { get; init; }

    /// <summary>
    /// Is this a property or a field ?
    /// </summary>
    public required bool IsProperty { get; init; }

    /// <summary>
    /// Whether the property is virtual.
    /// </summary>
    public required bool IsVirtual { get; init; }

    /// <summary>
    /// Whether the property is abstract.
    /// </summary>
    public required bool IsAbstract { get; init; }

    /// <summary>
    /// Whether the property is marked `required`.
    /// </summary>
    public required bool IsRequired { get; init; }

    /// <summary>
    /// Whether the property has an init-only set method.
    /// </summary>
    public required bool IsInitOnlySetter { get; init; }

    /// <inheritdoc />
    public bool Equals(ITypeDescriptor other) => other is PropertyDesc desc && Equals(desc);

    /// <inheritdoc />
    public string Name => MemberType.Name;

    /// <inheritdoc />
    public string FullyQualifiedName => MemberType.FullyQualifiedName;

    /// <inheritdoc />
    public bool IsValueType => MemberType.IsValueType;

    /// <inheritdoc />
    public TypeKind TypeKind => MemberType.TypeKind;

    /// <inheritdoc />
    public SpecialType SpecialType => MemberType.SpecialType;

    /// <inheritdoc />
    public bool CanBeNull => MemberType.CanBeNull;
}