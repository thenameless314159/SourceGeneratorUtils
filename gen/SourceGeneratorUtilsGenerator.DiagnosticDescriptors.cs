using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.SourceGeneration;

partial class SourceGeneratorUtilsGenerator
{
    public static class DiagnosticDescriptors
    {
        public static DiagnosticDescriptor FailedToEmitFromEmbeddedResources { get; } = new
        (
            id: "SGULIBB1000",
            title: "Could not emit generated type from embedded resources !",
            messageFormat: "Could not find embedded resource name '{0}' from generated type.",
            category: "SourceGeneratorUtils.SourceGeneration",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
    }
}