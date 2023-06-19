namespace SourceGeneratorUtils;

/// <inheritdoc/>
public abstract record AbstractTypeGenerationSpec : AbstractGenerationSpec
{
    /// <summary>
    /// Gets a list of type declarations to be used while generating the target type declaration.
    /// </summary>
    /// <remarks>
    /// The first item in the array (at index 0) must be the target type declaration.
    /// </remarks>
    public required ImmutableEquatableArray<string> TypeDeclarations { get; init; }
}