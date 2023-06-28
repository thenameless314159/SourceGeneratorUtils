using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using System.Reflection;

namespace SourceGeneratorUtils.SourceGeneration;

[Generator(LanguageNames.CSharp)]
public sealed partial class SourceGeneratorUtilsGenerator : IIncrementalGenerator
{
    public static readonly Assembly Assembly = typeof(SourceGeneratorUtilsGenerator).Assembly;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if LAUNCH_DEBUGGER
        System.Diagnostics.Debugger.Launch();
#endif
        IncrementalValueProvider<GenerationOptions> options = context.AnalyzerConfigOptionsProvider.Select(Parser.ParseGenerationOptions);

        IncrementalValueProvider<(SourceGenerationSpec?, ImmutableEquatableArray<DiagnosticInfo>)> sourceGenerationSpec = options
            .Select(static (options, cancellationToken) => 
            {
                Parser parser = new();
                SourceGenerationSpec? sourceGenSpec = parser.ParseSourceGenerationSpec(options, cancellationToken);
                ImmutableEquatableArray<DiagnosticInfo> diagnostics = parser.Diagnostics.ToImmutableEquatableArray();
                return (sourceGenSpec, diagnostics);
            })
            .WithTrackingName(nameof(SourceGenerationSpec));

        context.RegisterSourceOutput(sourceGenerationSpec, ReportDiagnosticsAndEmitSource);
    }

    private void ReportDiagnosticsAndEmitSource(SourceProductionContext sourceProductionContext, 
        (SourceGenerationSpec? SourceGenerationSpec, ImmutableEquatableArray<DiagnosticInfo> Diagnostics) input)
    {
        // Report any diagnostics ahead of emitting.
        foreach (DiagnosticInfo diagnostic in input.Diagnostics)
        {
            sourceProductionContext.ReportDiagnostic(diagnostic.CreateDiagnostic());
        }

        if (input.SourceGenerationSpec is null)
        {
            return;
        }

        OnSourceEmitting?.Invoke(ImmutableArray.Create(input.SourceGenerationSpec));
        Emitter emitter = new(sourceProductionContext);
        emitter.Emit(input.SourceGenerationSpec);
    }

    /// <summary>
    /// Instrumentation helper for unit tests.
    /// </summary>
    public Action<ImmutableArray<SourceGenerationSpec>>? OnSourceEmitting { get; init; }
}