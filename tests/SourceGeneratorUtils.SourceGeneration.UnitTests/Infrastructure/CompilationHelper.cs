using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using System.Reflection;

namespace SourceGeneratorUtils.SourceGeneration.UnitTests;

public static class CompilationHelper
{
    private static readonly GeneratorDriverOptions _generatorDriverOptions = new(disabledOutputs: IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true);
    private static readonly CSharpParseOptions _parseOptions = new(kind: SourceCodeKind.Regular, documentationMode: DocumentationMode.Parse);

    public static Compilation CreateCompilation(
        string source,
        string assemblyName = "TestAssembly",
        MetadataReference[]? additionalReferences = null,
        Func<CSharpParseOptions, CSharpParseOptions>? configureParseOptions = null)
    {
        List<MetadataReference> references = new()
        {
            MetadataReference.CreateFromFile(Assembly.GetAssembly(typeof(object))!.Location),
        };

        // Add additional references as needed.
        if (additionalReferences != null)
        {
            foreach (MetadataReference reference in additionalReferences)
            {
                references.Add(reference);
            }
        }

        var parseOptions = configureParseOptions?.Invoke(_parseOptions) ?? _parseOptions;
        return CSharpCompilation.Create(
            assemblyName,
            references: references.ToArray(),
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source, parseOptions) },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
    }

    public static CSharpGeneratorDriver CreateSourceGeneratorDriver(SourceGeneratorUtilsGenerator? generator = null)
    {
        generator ??= new();
        return CSharpGeneratorDriver.Create(
            generators: new[] { generator.AsSourceGenerator() },
            driverOptions: _generatorDriverOptions, 
            parseOptions: _parseOptions);
    }

    public static SourceGeneratorResult RunSourceGenerator(Compilation compilation)
    {
        ImmutableArray<SourceGenerationSpec> generatedSpecs = ImmutableArray<SourceGenerationSpec>.Empty;
        SourceGeneratorUtilsGenerator generator = new() { OnSourceEmitting = specs => generatedSpecs = specs };

        CSharpGeneratorDriver driver = CreateSourceGeneratorDriver(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outCompilation, out ImmutableArray<Diagnostic> diagnostics);

        return new()
        {
            Diagnostics = diagnostics,
            NewCompilation = outCompilation,
            SourceGenerationSpecs = generatedSpecs
        };
    }

    /// <summary>
    /// Create a default compilation for testing that uses one of the available emitted source.
    /// </summary>
    /// <returns>The compilation.</returns>
    public static Compilation CreateDefaultCompilation()
    {
        const string source = """
            using SourceGeneratorUtils;
            
            namespace Test
            {
                public static class TestClass
                {
                    public static string TestMethod()
                    {
                        SourceWriter writer = new();
                        writer.WriteLine("Hello, World!");
                        return writer.ToString();
                    }
                }
            }
            """;

        return CreateCompilation(source);
    }
}