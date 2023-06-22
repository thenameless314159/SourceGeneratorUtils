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
    /// <param name="targetMembers">The target type <see cref="PropertyDesc"/> members.</param>
    /// <param name="generatedTypes">The generated <see cref="ITypeDescriptor"/> types.</param>
    /// <returns>A new instance of <see cref="DefaultGenerationSpec"/>.</returns>
    public static DefaultGenerationSpec Create(
        TypeDesc target, 
        IEnumerable<PropertyDesc>? targetMembers = null,
        IEnumerable<ITypeDescriptor>? generatedTypes = null) 
        => new() 
        {
            Namespace = target.Namespace,
            TargetType = target.ToTypeRef(),
            TypeDeclarations = target.GetTypeDeclarationWithContainingTypes().ToImmutableEquatableArray(),
            TargetMembers = targetMembers?.ToImmutableEquatableArray() ?? ImmutableEquatableArray.Empty<PropertyDesc>(),
            GeneratedTypes = generatedTypes?.ToImmutableEquatableArray() ?? ImmutableEquatableArray.Empty<ITypeDescriptor>(),
        };

    /// <summary>
    /// Creates a new instance of <see cref="DefaultGenerationSpec"/> with <see cref="AbstractTypeGenerationSpec.TypeDeclarations"/> populated
    /// from the given <paramref name="target"/> <see cref="TypeDesc"/>.
    /// </summary>
    /// <remarks>
    /// This function takes arrays instead of <see cref="IEnumerable{T}"/> to avoid potential re-allocations.
    /// </remarks>
    /// <param name="target">The target <see cref="ITypeDescriptor"/>.</param>
    /// <param name="targetMembers">The target type <see cref="PropertyDesc"/> members.</param>
    /// <param name="generatedTypes">The generated <see cref="ITypeDescriptor"/> types.</param>
    /// <returns>A new instance of <see cref="DefaultGenerationSpec"/>.</returns>
    public static DefaultGenerationSpec CreateFrom(
        TypeDesc target,
        PropertyDesc[]? targetMembers = null,
        ITypeDescriptor[]? generatedTypes = null)
        => new()
        {
            Namespace = target.Namespace,
            TargetType = target.ToTypeRef(),
            TypeDeclarations = target.GetTypeDeclarationWithContainingTypes().ToImmutableEquatableArray(),
            TargetMembers = ImmutableEquatableArray.Create(targetMembers ?? Array.Empty<PropertyDesc>()),
            GeneratedTypes = ImmutableEquatableArray.Create(generatedTypes ?? Array.Empty<ITypeDescriptor>()),
        };

    /// <summary>
    /// Gets the target type ref descriptor.
    /// </summary>
    public required TypeRef TargetType { get; init; }

    /// <summary>
    /// Gets the target type members descriptors.
    /// </summary>
    public required ImmutableEquatableArray<PropertyDesc> TargetMembers { get; init; }

    /// <summary>
    /// Gets an array of <see cref="ITypeDescriptor"/> that represents all the types that needs a source file to be generated for.
    /// </summary>
    public ImmutableEquatableArray<ITypeDescriptor> GeneratedTypes { get; init; } = ImmutableEquatableArray<ITypeDescriptor>.Empty;
}