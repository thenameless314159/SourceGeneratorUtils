using static SourceGeneratorUtils.SourceGeneration.EmbeddedResourcesStore;
using static SourceGeneratorUtils.SourceGeneration.WellKnownStrings;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.SourceGeneration;

partial class SourceGeneratorUtilsGenerator
{
    // The source generator requires NRT, init-only property and file-scoped namespaces support.
    // review: may refactor lib without file-scoped namespaces to support C# 9.0
    private const LanguageVersion MinimumSupportedLanguageVersion = LanguageVersion.CSharp10;

    private sealed class Parser
    {
        public static GenerationOptions ParseGenerationOptions(AnalyzerConfigOptionsProvider optionsProvider, CancellationToken cancellationToken)
        {
            optionsProvider.TryGetGlobalOptionsValue(MakeGeneratedTypesInternalBuildProperty, out bool makeGeneratedTypesInternal);

            ImmutableEquatableArray<string> includedResources = ImmutableEquatableArray.Empty<string>();
            if (optionsProvider.TryGetGlobalOptionsValue(IncludedTypesBuildProperty, out ImmutableEquatableArray<string>? msBuildPropertyValue))
            {
                includedResources = msBuildPropertyValue;
            }

            ImmutableEquatableArray<string> excludedResources = ImmutableEquatableArray.Empty<string>();
            if (optionsProvider.TryGetGlobalOptionsValue(ExcludedTypesBuildProperty, out msBuildPropertyValue))
            {
                excludedResources = msBuildPropertyValue;
            }

            cancellationToken.ThrowIfCancellationRequested();

            return new() {
                MakeGeneratedTypesInternal = makeGeneratedTypesInternal,
                IncludeResources = includedResources, ExcludeResources = excludedResources
            };
        }

        public List<DiagnosticInfo> Diagnostics { get; } = new();

        private readonly KnownTypeSymbols _knownSymbols;

        public Parser(KnownTypeSymbols knownSymbols) 
            => _knownSymbols = knownSymbols;

        public SourceGenerationSpec? ParseSourceGenerationSpec(GenerationOptions options)
        {
            LanguageVersion? langVersion = _knownSymbols.Compilation.GetLanguageVersion();
            if (langVersion is null or < MinimumSupportedLanguageVersion)
            {
                // Unsupported lang version should be the first (and only) diagnostic emitted by the generator.
                ReportDiagnostic(DiagnosticDescriptors.UnsupportedLanguageVersion, null, langVersion?.ToDisplayString(), MinimumSupportedLanguageVersion.ToDisplayString());
                return null;
            }

            ImmutableEquatableArray<string> resourcesToInclude = GetResourcesToInclude(options).ToImmutableEquatableArray();
            if (resourcesToInclude.Count == 0)
            {
                ReportDiagnostic(DiagnosticDescriptors.NoTypeToEmit, null);
                return null;
            }

            return new()
            {
                ResourcesToGenerate = resourcesToInclude,
                UseInternalTypes = options.MakeGeneratedTypesInternal
            };
        }

        private IEnumerable<string> GetResourcesToInclude(GenerationOptions options)
        {
            foreach (string resourceName in FileNamesByResourceName.Keys)
            {
                bool isIncluded = IsResourceDefined(resourceName, options.IncludeResources);
                bool isExcluded = IsResourceDefined(resourceName, options.ExcludeResources);
                
                // Exclude if it isn't included
                if (isExcluded && !isIncluded)
                {
                    continue;
                }

                yield return resourceName;
            }
            
            static bool IsResourceDefined(string resourceName, ImmutableEquatableArray<string> resources)
            {
                if (resources.Count == 0) return false;
                if (resources[0] == "*") return true;

                foreach (string resource in resources)
                {
                    // review: should check without this assembly namespace prefix and without the extension
                    if (resource == resourceName || resourceName.Contains(resource))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void ReportDiagnostic(DiagnosticDescriptor descriptor, Location? location, params object?[]? messageArgs)
        {
            Diagnostics.Add(new DiagnosticInfo
            {
                Descriptor = descriptor,
                Location = location.GetTrimmedLocation(),
                MessageArgs = messageArgs ?? Array.Empty<object?>(),
            });
        }
    }
}
