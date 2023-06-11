namespace SourceGeneratorUtils;

public record SourceFileGenOptions
{
    public bool UseCombinedAttributes { get; init; }
    public bool UseFileScopedNamespace { get; init; }
    public bool EnableNullableWarnings { get; init; }
    public bool EnableNullableAnnotations { get; init; } = true;
    public Func<SourceWriter>? WriterFactory { get; init; } = null;

    public string? DefaultBaseType { get; init; }
    public IReadOnlyList<string> DefaultInterfaces { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> DefaultAttributes { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> DefaultUsingDirectives { get; init; } = Array.Empty<string>();

    public IReadOnlyList<ISourceBlockGenerator> BlockGenerators { get; init; } = Array.Empty<ISourceBlockGenerator>();
}