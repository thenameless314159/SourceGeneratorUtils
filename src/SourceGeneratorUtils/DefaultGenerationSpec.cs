namespace SourceGeneratorUtils;

/// <summary>
/// Specifications for generating source code of a given <see cref="ITypeDescriptor"/>.
/// </summary>
public sealed record DefaultGenerationSpec : AbstractTypeGenerationSpec
{
    /// <summary>
    /// Creates a new instance of <see cref="DefaultGenerationSpec"/> with <see cref="AbstractTypeGenerationSpec.TypeDeclarations"/> populated
    /// from the given <paramref name="target"/> <see cref="TypeDesc"/>.
    /// </summary>
    /// <param name="target">The target <see cref="ITypeDescriptor"/>.</param>
    /// <param name="descriptors">The generated types.</param>
    /// <returns></returns>
    public static DefaultGenerationSpec CreateFrom(TypeDesc target, params ITypeDescriptor[] descriptors) => new()
    {
        TargetType = target,
        Namespace = target.Namespace,
        GeneratedTypes = ImmutableEquatableArray.Create(descriptors),
        TypeDeclarations = new ImmutableEquatableArray<string>(target.GetTypeDeclarationWithContainingTypes()),
    };

    /// <summary>
    /// Gets the target type descriptor.
    /// Would typically either be a <see cref="TypeRef"/> or a <see cref="TypeDesc"/>.
    /// </summary>
    public required ITypeDescriptor TargetType { get; init; }

    /// <summary>
    /// Gets an array of <see cref="ITypeDescriptor"/> that represents all the types that needs a source file to be generated for.
    /// </summary>
    public ImmutableEquatableArray<ITypeDescriptor> GeneratedTypes { get; init; } = ImmutableEquatableArray<ITypeDescriptor>.Empty;
}