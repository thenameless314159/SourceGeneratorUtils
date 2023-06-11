namespace SourceGeneratorUtils;

public interface ISourceBlockGenerator
{
    IEnumerable<string> GetImportedNamespaces(SourceWritingContext context);
    IEnumerable<string> GetImplementedInterfaces(SourceWritingContext context);

    void GenerateBlock(SourceWriter writer, SourceWritingContext context);
}