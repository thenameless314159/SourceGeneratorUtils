namespace SourceGeneratorUtils.SourceGeneration;

internal sealed record GenerationOptions
{
    public required bool MakePublicGeneratedTypesInternal { get; init; }
    public required ImmutableEquatableArray<string> ExcludedTypes { get; init; }
}