namespace SourceGeneratorUtils.SourceGeneration;

internal sealed record SourceGenerationSpec
{
    public required bool MakePublicGeneratedTypesInternal { get; init; }
    public required ImmutableEquatableArray<string> TypesToGenerate { get; init; }
}