namespace SourceGeneratorUtils;

/// <summary>
/// Represents a type specification used by <see cref="CSharpSourceGenerator{TDescriptor}"/>
/// to describe the target type to generate sources for.
/// </summary>
public interface ITypeSpec
{
    /// <summary>
    /// Gets the name of the type.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the type of the type. Can be "class", "struct", "record", etc.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Gets the namespace of the type.
    /// </summary>
    string Namespace { get; }

    /// <summary>
    /// Gets the name of the base type if any.
    /// </summary>
    string? BaseTypeName { get; }

    /// <summary>
    /// Gets the accessibility of the type if any. Can be "public", "internal", etc.
    /// </summary>
    string? Accessibility { get; }

    /// <summary>
    /// Gets the attributes of the type.
    /// </summary>
    IReadOnlyList<string> Attributes { get; }

    /// <summary>
    /// Gets the types that contain this type.
    /// </summary>
    IReadOnlyList<ITypeSpec> ContainingTypes { get; }
}