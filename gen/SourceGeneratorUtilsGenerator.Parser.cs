using EnumHelper.SourceGenerator;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SourceGeneratorUtils.SourceGeneration;

partial class SourceGeneratorUtilsGenerator
{
    public sealed class Parser
    {
        public static GenerationOptions ParseGenerationOptions(AnalyzerConfigOptionsProvider optionsProvider, CancellationToken cancellationToken)
        {
            return new GenerationOptions
            {
                MakePublicGeneratedTypesInternal = false,
                ExcludedTypes = ImmutableEquatableArray.Empty<string>()
            };
        }

        public List<DiagnosticInfo> Diagnostics { get; } = new();

        private readonly KnownTypeSymbols _knownSymbols;

        public Parser(KnownTypeSymbols knownSymbols) 
            => _knownSymbols = knownSymbols;

        public SourceGenerationSpec? ParseSourceGenerationSpec(GenerationOptions options, CancellationToken cancellationToken)
        {
            return new()
            {
                MakePublicGeneratedTypesInternal = options.MakePublicGeneratedTypesInternal,
                TypesToGenerate = EmbeddedResourcesStore.FileNamesByResourceName.Keys.ToImmutableEquatableArray()
            };
        }
    }
}
