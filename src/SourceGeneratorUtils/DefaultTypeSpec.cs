namespace SourceGeneratorUtils;

/// <summary>
/// Represents a default implementation of <see cref="ITypeSpec"/>.
/// </summary>
/// <param name="Name">The name of the type.</param>
/// <param name="Type">The type of the type. Can be "class", "struct", "record", etc.</param>
/// <param name="Namespace">The namespace of the type.</param>
/// <param name="BaseTypeName">The name of the base type if any.</param>
/// <param name="Accessibility">The accessibility of the type if any. Can be "public", "internal", etc.</param>
public sealed record DefaultTypeSpec
    (string Name, string Type, string Namespace, string? BaseTypeName = null, string? Accessibility = null) 
    : BaseTypeSpec(Name, Type, Namespace, BaseTypeName, Accessibility);