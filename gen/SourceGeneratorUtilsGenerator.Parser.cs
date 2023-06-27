using static SourceGeneratorUtils.SourceGeneration.EmbeddedResourcesStore;
using static SourceGeneratorUtils.SourceGeneration.WellKnownStrings;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.SourceGeneration;

partial class SourceGeneratorUtilsGenerator
{
    public sealed class Parser
    {
        public static GenerationOptions ParseGenerationOptions(AnalyzerConfigOptionsProvider optionsProvider, CancellationToken _)
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

            return new() {
                MakeGeneratedTypesInternal = makeGeneratedTypesInternal,
                IncludeResources = includedResources, ExcludeResources = excludedResources
            };
        }

        public List<DiagnosticInfo> Diagnostics { get; } = new();

        private readonly KnownTypeSymbols _knownSymbols;

        public Parser(KnownTypeSymbols knownSymbols) 
            => _knownSymbols = knownSymbols;

        public SourceGenerationSpec? ParseSourceGenerationSpec(GenerationOptions options, CancellationToken _)
        {
            ImmutableEquatableArray<string> resourcesToInclude = GetResourcesToInclude(options).ToImmutableEquatableArray();

            if (resourcesToInclude.Count == 0)
            {
                ReportDiagnostic(DiagnosticDescriptors.NoTypeToEmit, null);
                return null;
            }

            return new()
            {
                TypesToGenerate = resourcesToInclude,
                UseInternalTypes = options.MakeGeneratedTypesInternal
            };
        }

        private IEnumerable<string> GetResourcesToInclude(GenerationOptions options)
        {
            foreach (string resourceName in FileNamesByResourceName.Keys)
            {
                // Skip resources that are excluded if they're not included.
                if (IsResourceDefined(resourceName, options.ExcludeResources) &&
                    !IsResourceDefined(resourceName, options.IncludeResources))
                {
                    continue;
                }

                // Skip already existing polyfills.
                if (resourceName.Contains(nameof(System)) &&
                    _knownSymbols.IsPolyfillDefined(FileNamesByResourceName[resourceName]))
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
                    if (resourceName == resource || resourceName.Contains(resource))
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
