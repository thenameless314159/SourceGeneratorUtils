using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SourceGeneratorUtils.SourceGeneration.UnitTests;

public class SourceGeneratorUtilsGeneratorDiagnosticsTests
{
    [Fact]
    public void ExcludingAllResourcesCausesDiagnostic()
    {
        Compilation compilation = CreateDefaultCompilation();
        SourceGeneratorResult result = RunSourceGenerator(compilation, new()
        {
            { WellKnownStrings.ExcludedTypesBuildProperty, "*"}
        });

        False(result.Diagnostics.IsDefaultOrEmpty);
        Equal(SourceGeneratorUtilsGenerator.DiagnosticDescriptors.NoTypeToEmit, result.Diagnostics[0].Descriptor);
    }

    [Fact]
    public void IncludedResourcesWithEverythingElseExcludedWorks()
    {
        Compilation compilation = CreateDefaultCompilation();
        SourceGeneratorResult result = RunSourceGenerator(compilation, new()
        {
            { WellKnownStrings.IncludedTypesBuildProperty, string.Join(',', EmbeddedResourcesStore.FileNamesByResourceName.Keys) },
            { WellKnownStrings.ExcludedTypesBuildProperty, "*"}
        });

        True(result.Diagnostics.IsDefaultOrEmpty);
        foreach (string resourceName in EmbeddedResourcesStore.FileNamesByResourceName.Keys)
            result.AssertContainsResourceName(resourceName);
    }

    [Theory]
    [InlineData(LanguageVersion.Default)]
    [InlineData(LanguageVersion.Preview)]
    [InlineData(LanguageVersion.Latest)]
    [InlineData(LanguageVersion.LatestMajor)]
    [InlineData(LanguageVersion.CSharp10)]
    [InlineData(LanguageVersion.CSharp11)]
    public void SupportedLanguageVersions_SucceedCompilation(LanguageVersion langVersion)
    {
        Compilation compilation = CreateCompilation(DefaultSource, 
            configureParseOptions: o => o.WithLanguageVersion(langVersion));

        SourceGeneratorResult result = RunSourceGenerator(compilation);
        Empty(result.Diagnostics);
    }

    [Theory]
    [InlineData(LanguageVersion.CSharp1)]
    [InlineData(LanguageVersion.CSharp2)]
    [InlineData(LanguageVersion.CSharp3)]
    [InlineData(LanguageVersion.CSharp7)]
    [InlineData(LanguageVersion.CSharp7_3)]
    [InlineData(LanguageVersion.CSharp8)]
    [InlineData(LanguageVersion.CSharp9)]
    public void UnsupportedLanguageVersions_FailCompilation(LanguageVersion langVersion)
    {
        Compilation compilation = CreateCompilation(DefaultSource,
            configureParseOptions: o => o.WithLanguageVersion(langVersion));

        SourceGeneratorResult result = RunSourceGenerator(compilation);
        Equal(SourceGeneratorUtilsGenerator.DiagnosticDescriptors.UnsupportedLanguageVersion, result.Diagnostics[0].Descriptor);
    }
}