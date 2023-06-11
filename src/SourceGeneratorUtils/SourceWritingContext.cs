namespace SourceGeneratorUtils;

public record SourceWritingContext
{
    public required ITypeSpec Target { get; init; }
    public required SourceFileGenOptions Options { get; init; }
    public required IReadOnlyDictionary<string, ITypeSpec> Types { get; init; }
}