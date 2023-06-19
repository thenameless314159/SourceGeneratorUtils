using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils;

/// <summary>
/// Models common specifications for a type member (property or field).
/// </summary>
/// <remarks>
/// For source generators, type needs to be cacheable as a Roslyn incremental value so it must be
///
/// 1) immutable and
/// 2) implement structural (pointwise) equality comparison.
///
/// We can get these properties for free provided that we
///
/// a) define the type as an immutable C# record and
/// b) ensure all nested members are also immutable and implement structural equality.
///
/// As an immutable record, this type should encapsulate most logic to handle comparison between members therefore
/// it should be safe to add new members to this type without risking to break incremental caching in your source generator.
/// (considering that this record as a base would cover most common cases for most member definition changes and that
/// new members would be readonly properties)
/// </remarks>
public record TypeMemberDesc
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
    public required ITypeDescriptor MemberType { get; init; }

    /// <summary>
    /// Gets or init a reference to the declaring type of the property.
    /// </summary>
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
    /// Is this a property or a field ?
    /// </summary>
    public required bool IsProperty { get; init; }

    /// <summary>
    /// Whether the property is virtual.
    /// </summary>
    public required bool IsVirtual { get; init; }

    /// <summary>
    /// Whether the property is virtual.
    /// </summary>
    public required bool IsAbstract { get; init; }

    /// <summary>
    /// Whether the property has a set method.
    /// </summary>
    public required bool IsReadOnly { get; init; }

    /// <summary>
    /// Whether the property is marked `required`.
    /// </summary>
    public required bool IsRequired { get; init; }

    /// <summary>
    /// Whether the property has an init-only set method.
    /// </summary>
    public required bool IsInitOnlySetter { get; init; }
}