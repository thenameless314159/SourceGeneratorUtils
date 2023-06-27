using static SourceGeneratorUtils.SourceGeneration.EmbeddedResourcesStore;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Text;

namespace SourceGeneratorUtils.SourceGeneration;

partial class SourceGeneratorUtilsGenerator
{
    public sealed class Emitter
    {
        private readonly SourceProductionContext _context;

        public Emitter(SourceProductionContext context)
            => _context = context;

        public void Emit(SourceGenerationSpec sourceGenerationSpec)
        {
            foreach (string resourceName in sourceGenerationSpec.TypesToGenerate)
            {
                try
                {
                    string resourceContent = GetEmbeddedResourceContent(resourceName, cacheResourceContent: true);
                    SourceText sourceText = SourceText.From(resourceContent, Encoding.UTF8);
                    _context.AddSource(FileNamesByResourceName[resourceName], sourceText);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    _context.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.FailedToEmitFromEmbeddedResources,
                            location: null,
                            resourceName));
                }
            }
        }
    }
}