namespace SourceGeneratorUtils;

/// <summary>
/// Abstracts the writing of a source block within a <see cref="CSharpSourceGenerator{TDescriptor}"/>.
/// </summary>
/// <typeparam name="TDescriptor">The type of the target descriptor to generate a source file for.</typeparam>
public abstract class CSharpSourceBlockWriter<TDescriptor>
{
    /// <summary>
    /// Writes the source block to the given <paramref name="writer"/> within the given <paramref name="target"/> body.
    /// </summary>
    /// <param name="writer">The <see cref="SourceWriter"/> to write to.</param>
    /// <param name="target">The target <see cref="ITypeSpec"/>.</param>
    /// <param name="options">The <see cref="SourceFileGenOptions"/> to use.</param>
    public abstract void WriteTo(SourceWriter writer, TDescriptor target, SourceFileGenOptions options);

    /// <summary>
    /// Gets the namespaces to import within the given target descriptor source files.
    /// These will be added as using directives and may be a using declaration or simply a namespace.
    /// </summary>
    /// <param name="target">The target <see cref="ITypeSpec"/>.</param>
    /// <param name="options">The <see cref="SourceFileGenOptions"/> to use.</param>
    /// <returns>An enumerable of the namespaces to import.</returns>
    public virtual IEnumerable<string> GetImportedNamespaces(TDescriptor target, SourceFileGenOptions options) => Enumerable.Empty<string>();

    /// <summary>
    /// Gets the interfaces that the given target descriptor must implement within its generated source files.
    /// </summary>
    /// <param name="target">The target <see cref="ITypeSpec"/>.</param>
    /// <param name="options">The <see cref="SourceFileGenOptions"/> to use.</param>
    /// <returns>An enumerable of the interfaces to implement.</returns>
    public virtual IEnumerable<string> GetImplementedInterfaces(TDescriptor target, SourceFileGenOptions options) => Enumerable.Empty<string>();
}