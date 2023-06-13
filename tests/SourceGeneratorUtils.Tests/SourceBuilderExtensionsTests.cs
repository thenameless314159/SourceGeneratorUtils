namespace SourceGeneratorUtils.Tests;

public class SourceBuilderExtensionsTests
{
    private static readonly DefaultTypeSpec[] _descriptors =
    {
        new("Test", "class", "SourceGeneratorUtils.Tests"),
        new("Test2", "partial class", "SourceGeneratorUtils.Tests"),
        new("NamelessClass", "partial class", "SourceGeneratorUtils.Tests"),
    };

    [Fact]
    public void Register_ShouldPopulateSourceBuilder_WithGeneratedSourceFile()
    {
        var srcFileGenerator = new TestSourceFileGenerator();
        var builder = new SourceBuilder();

        builder.Register(srcFileGenerator, _descriptors[0]);
        True(builder.SourceFiles.TryGetValue("Test", out var generatedFile));
        Equal("Hello There!" + Environment.NewLine, generatedFile.ToString());
    }

    [Fact]
    public void PopulateWith_Relatives_ShouldPopulateSourceBuilder_WithGeneratedSourceFiles()
    {
        var srcFileGenerator = new TestSourceFileGenerator();
        var builder = new SourceBuilder();

        builder.PopulateWith(srcFileGenerator, _descriptors, null);
        foreach (var desc in _descriptors)
        {
            True(builder.SourceFiles.TryGetValue(desc.Name, out var generatedFile));
            Equal("Hello There!" + Environment.NewLine, generatedFile.ToString());
        }

        builder = new SourceBuilder();
        builder.PopulateWith(srcFileGenerator, _descriptors.ToList(), null);
        foreach (var desc in _descriptors)
        {
            True(builder.SourceFiles.TryGetValue(desc.Name, out var generatedFile));
            Equal("Hello There!" + Environment.NewLine, generatedFile.ToString());
        }
    }

    [Theory, InlineData(true), InlineData(false)]
    public void PopulateWith_NoRelatives_ShouldPopulateSourceBuilder_WithGeneratedSourceFiles(bool makeRelativesDict)
    {
        var srcFileGenerator = new TestSourceFileGenerator { RelativesExpected = makeRelativesDict };
        var builder = new SourceBuilder();

        builder.PopulateWith(srcFileGenerator, _descriptors, makeRelativesDict);
        foreach (var desc in _descriptors)
        {
            True(builder.SourceFiles.TryGetValue(desc.Name, out var generatedFile));
            Equal("Hello There!" + Environment.NewLine, generatedFile.ToString());
        }
    }

    private class TestSourceFileGenerator : ISourceFileGenerator<DefaultTypeSpec>
    {
        public bool RelativesExpected { get; set; }

        public SourceFileDescriptor GenerateSource(in DefaultTypeSpec target,
            IReadOnlyDictionary<string, DefaultTypeSpec>? relatives = null)
        {
            True(!RelativesExpected || relatives != null);
            return new(target.Name, new SourceWriter().WriteLine("Hello There!"));
        }
    }
}