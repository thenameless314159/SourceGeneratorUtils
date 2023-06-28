using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace SourceGeneratorUtils.SourceGeneration.UnitTests;

public sealed record SourceGeneratorResult
{
    public ImmutableArray<SourceGenerationSpec> SourceGenerationSpecs { get; set; }
    public ImmutableArray<Diagnostic> Diagnostics { get; set; }
    public required Compilation NewCompilation { get; set; }

    public IEnumerable<string> AllGeneratedResources => SourceGenerationSpecs
        .SelectMany(static ctx => ctx.ResourcesToGenerate);

    public void AssertContainsResourceName(string resourceName)
        => Contains(
            AllGeneratedResources,
            resource => resource == resourceName);
}