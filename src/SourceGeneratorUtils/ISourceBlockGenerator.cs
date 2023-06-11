namespace SourceGeneratorUtils;

/// <summary>
/// Generates a source block for the given context.
/// </summary>
public interface ISourceBlockGenerator
{
    /// <summary>
    /// Gets an enumerable of the interfaces that the generated type should implement.
    /// </summary>
    /// <param name="context">The source writing context.</param>
    /// <returns>An enumerable of the interfaces that the generated type should implement.</returns>
    IEnumerable<string> GetImplementedInterfaces(in SourceWritingContext context);

    /// <summary>
    /// Gets an enumerable of the namespaces that the generated type should import.
    /// </summary>
    /// <param name="context">The source writing context.</param>
    /// <returns>An enumerable of the namespaces that the generated type should import.</returns>
    IEnumerable<string> GetImportedNamespaces(in SourceWritingContext context);

    /// <summary>
    /// Generates a source block for the given context.
    /// </summary>
    /// <param name="writer">The target source writer.</param>
    /// <param name="context">The source writing context.</param>
    void GenerateBlock(SourceWriter writer, in SourceWritingContext context);
}