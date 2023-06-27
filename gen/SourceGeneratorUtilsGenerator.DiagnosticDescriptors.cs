using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.SourceGeneration;

partial class SourceGeneratorUtilsGenerator
{
    public static class DiagnosticDescriptors
    {
        public static DiagnosticDescriptor NoTypeToEmit { get; } = new
        (
            id: "SGULIBB1002",
            title: "Include at least one resource or remove this dependency !",
            messageFormat: "All types from this assembly have been excluded, either include at least one type or remove this dependency.",
            category: "SourceGeneratorUtils.SourceGeneration",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static DiagnosticDescriptor FailedToEmitFromEmbeddedResources { get; } = new
        (
            id: "SGULIBB1001",
            title: "Could not emit generated type from embedded resources !",
            messageFormat: "Could not find embedded resource name '{0}' from generated type.",
            category: "SourceGeneratorUtils.SourceGeneration",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
    }
}