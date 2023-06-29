using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.SourceGeneration;

partial class SourceGeneratorUtilsGenerator
{
    public static class DiagnosticDescriptors
    { 
        public static DiagnosticDescriptor UnsupportedLanguageVersion { get; } = new
        (
            id: "SGULIB1000",
            title: "C# language version not supported by the source generator.",
            messageFormat: "The SourceGeneratorUtils.SourceGeneration source generator is not available in C# '{0}'. Please use language version {1} or greater.",
            category: "SourceGeneratorUtils.SourceGeneration",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

        public static DiagnosticDescriptor NoTypeToEmit { get; } = new
        (
            id: "SGULIB1001",
            title: "Include at least one resource or remove this dependency !",
            messageFormat: "All types from this assembly have been excluded, either include at least one type or remove this dependency.",
            category: "SourceGeneratorUtils.SourceGeneration",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );
    }
}