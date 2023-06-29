using static SourceGeneratorUtils.SourceGeneration.EmbeddedResourcesStore;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Text;

namespace SourceGeneratorUtils.SourceGeneration;

partial class SourceGeneratorUtilsGenerator
{
    private sealed class Emitter
    {
        private readonly SourceProductionContext _context;

        public Emitter(SourceProductionContext context)
            => _context = context;

        public void Emit(SourceGenerationSpec sourceGenerationSpec)
        {
            foreach (string resourceName in sourceGenerationSpec.ResourcesToGenerate)
            {
                string resourceContent = resourceName.Contains(nameof(System))
                    ? GetEmbeddedResourceContent(resourceName, cacheResourceContent: true)
                    : GetModifiedEmbeddedResourceContent(resourceName, sourceGenerationSpec.UseInternalTypes);

                SourceText sourceText = SourceText.From(resourceContent, Encoding.UTF8);
                _context.AddSource(FileNamesByResourceName[resourceName], sourceText);
            }
        }

        // review: modified resources must be cached, generated code and exclude from coverage attributes must be applied
        private static string GetModifiedEmbeddedResourceContent(string resourceName, bool useInternalTypes)
        {
            string resourceContent = GetEmbeddedResourceContent(resourceName, cacheResourceContent: false);
            return $"""
                {WellKnownStrings.SourceFileHeader}
                
                {(useInternalTypes ? resourceContent.Replace("public", "internal") : resourceContent)}
                """;
        }
    }
}