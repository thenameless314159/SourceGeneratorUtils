using static SourceGeneratorUtils.SourceGeneration.EmbeddedResourcesStore;
using static SourceGeneratorUtils.SourceGeneration.WellKnownStrings;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.SourceGeneration;

partial class SourceGeneratorUtilsGenerator
{
    internal sealed class Parser
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
                ResourcesToGenerate = resourcesToInclude,
                UseInternalTypes = options.MakeGeneratedTypesInternal
            };
        }

        private IEnumerable<string> GetResourcesToInclude(GenerationOptions options)
        {
            foreach (KeyValuePair<string, string> kvp in FileNamesByResourceName)
            {
                bool isIncluded = IsResourceDefined(kvp.Key, options.IncludeResources);
                bool isExcluded = IsResourceDefined(kvp.Key, options.ExcludeResources);
                bool isPolyfill = kvp.Value.StartsWith(nameof(System));

                // Exclude if it isn't included nor a required polyfill
                if (isExcluded && !isIncluded && !isPolyfill)
                {
                    continue;
                }

                yield return kvp.Key;
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
