using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using System.Reflection;

namespace SourceGeneratorUtils.SourceGeneration.UnitTests;

public static class CompilationHelper
{
    private static readonly GeneratorDriverOptions _generatorDriverOptions = new(disabledOutputs: IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true);
    private static readonly CSharpCompilationOptions _compileOptions = new(OutputKind.DynamicallyLinkedLibrary);
    private static readonly CSharpParseOptions _parseOptions = CreateParseOptions();

    public static CSharpParseOptions CreateParseOptions(LanguageVersion? version = null, DocumentationMode? documentationMode = null)
    {
        return new CSharpParseOptions(
            kind: SourceCodeKind.Regular,
            languageVersion: version ?? LanguageVersion.CSharp10, // C# 10 is the minimum supported lang version by the source generator.
            documentationMode: documentationMode ?? DocumentationMode.Parse);
    }

    public static Compilation CreateCompilation(
        string source,
        string assemblyName = "TestAssembly",
        MetadataReference[]? additionalReferences = null,
        Func<CSharpParseOptions, CSharpParseOptions>? configureParseOptions = null,
        Func<CSharpCompilationOptions, CSharpCompilationOptions>? configureCompileOptions = null)
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
        var compileOptions = configureCompileOptions?.Invoke(_compileOptions) ?? _compileOptions;

        return CSharpCompilation.Create(assemblyName,
            options: compileOptions,
            references: references.ToArray(),
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source, parseOptions) }
        );
    }

    public static CSharpGeneratorDriver CreateSourceGeneratorDriver(
        Compilation compilation, 
        SourceGeneratorUtilsGenerator? generator = null, 
        Dictionary<string, string>? buildProperties = null)
    {
        generator ??= new();
        CSharpParseOptions parseOptions = compilation.SyntaxTrees
            .OfType<CSharpSyntaxTree>()
            .Select(tree => tree.Options)
            .FirstOrDefault() ?? _parseOptions;

        return CSharpGeneratorDriver.Create(
            optionsProvider: buildProperties is null ? null : new TestAnalyzerConfigOptionsProvider(buildProperties),
            generators: new[] { generator.AsSourceGenerator() },
            driverOptions: _generatorDriverOptions, 
            parseOptions: parseOptions);
    }

    public static SourceGeneratorResult RunSourceGenerator(Compilation compilation, Dictionary<string, string>? buildProperties = null)
    {
        ImmutableArray<SourceGenerationSpec> generatedSpecs = ImmutableArray<SourceGenerationSpec>.Empty;
        SourceGeneratorUtilsGenerator generator = new() { OnSourceEmitting = specs => generatedSpecs = specs };

        CSharpGeneratorDriver driver = CreateSourceGeneratorDriver(compilation, generator, buildProperties);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outCompilation, out ImmutableArray<Diagnostic> diagnostics);

        return new()
        {
            Diagnostics = diagnostics,
            NewCompilation = outCompilation,
            SourceGenerationSpecs = generatedSpecs
        };
    }

    // use the same source for most tests since we're not parsing any syntax trees
    public const string DefaultSource = """
            using SourceGeneratorUtils;
            
            namespace Test
            {
                public static class TestClass
                {
                    public static string TestMethod()
                    {
                        SourceWriter writer = new SourceWriter();
                        writer.WriteLine("Hello, World!");
                        return writer.ToString();
                    }
                }
            }
            """;

    /// <summary>
    /// Create a default compilation for testing that uses one of the available emitted source.
    /// </summary>
    /// <returns>The compilation.</returns>
    public static Compilation CreateDefaultCompilation() => CreateCompilation(DefaultSource);
}