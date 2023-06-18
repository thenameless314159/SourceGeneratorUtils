namespace SourceGeneratorUtils;

/// <summary>
/// A specification for generating source code of a given <see cref="ITypeDescriptor"/>.
/// </summary>
public sealed record TypeGenerationSpec : AbstractGenerationSpec
{
    /// <summary>
    /// Creates a new instance of <see cref="TypeGenerationSpec"/> with <see cref="TypeDeclarations"/> populated
    /// from the given <paramref name="target"/>.
    /// </summary>
    /// <param name="target">The target <see cref="ITypeDescriptor"/>.</param>
    /// <param name="descriptors">The generated types.</param>
    /// <returns></returns>
    public static TypeGenerationSpec CreateFrom(TypeDesc target, params ITypeDescriptor[] descriptors) => new()
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
    /// Gets a list of type declarations to be used while generating the target type declaration.
    /// </summary>
    /// <remarks>
    /// The first item in the array (at index 0) must be the target type declaration.
    /// This array can be populated either manually (if used with <see cref="TypeRef"/>, typically in source generators)
    /// or by the <see cref="CreateFrom(TypeDesc,ITypeDescriptor[])"/> static factory method.
    /// </remarks>
    public ImmutableEquatableArray<string> TypeDeclarations { get; init; } = ImmutableEquatableArray<string>.Empty;

    /// <summary>
    /// Gets an array of <see cref="ITypeDescriptor"/> that represents all the types that needs a source file to be generated for.
    /// </summary>
    public ImmutableEquatableArray<ITypeDescriptor> GeneratedTypes { get; init; } = ImmutableEquatableArray<ITypeDescriptor>.Empty;
}