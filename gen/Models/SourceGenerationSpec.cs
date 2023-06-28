namespace SourceGeneratorUtils.SourceGeneration;

internal sealed record SourceGenerationSpec
{
    public required bool UseInternalTypes { get; init; }
    public required ImmutableEquatableArray<string> ResourcesToGenerate { get; init; }
}