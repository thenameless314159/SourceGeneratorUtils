namespace SourceGeneratorUtils;

public sealed record SourceFileGenOptions
{
    public bool UseInitOnlyProperties { get; init; }
    public bool UseCombinedAttributes { get; init; }
    public bool UseFileScopedNamespace { get; init; }
    public bool EnableNullableWarnings { get; init; }
    public bool EnableNullableAnnotations { get; init; } = true;
    public Func<SourceWriter>? WriterFactory { get; init; } = null;
    public IReadOnlyList<ISourceBlockGenerator> BlockGenerators { get; init; } = Array.Empty<ISourceBlockGenerator>();
}