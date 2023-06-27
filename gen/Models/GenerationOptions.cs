namespace SourceGeneratorUtils.SourceGeneration;

internal sealed record GenerationOptions
{
    public required bool MakeGeneratedTypesInternal { get; init; }
    public required ImmutableEquatableArray<string> IncludeResources { get; init; }
    public required ImmutableEquatableArray<string> ExcludeResources { get; init; }
}