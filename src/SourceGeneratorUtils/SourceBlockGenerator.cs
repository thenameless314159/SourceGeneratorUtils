namespace SourceGeneratorUtils;

public abstract class SourceBlockGenerator : ISourceBlockGenerator
{
    /// <inheritdoc />
    public abstract void GenerateBlock(SourceWriter writer, in SourceWritingContext context);

    /// <inheritdoc />
    public virtual IEnumerable<string> GetImportedNamespaces(in SourceWritingContext context) => Enumerable.Empty<string>();

    /// <inheritdoc />
    public virtual IEnumerable<string> GetImplementedInterfaces(in SourceWritingContext context) => Enumerable.Empty<string>();
}