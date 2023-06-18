namespace SourceGeneratorUtils.Tests;

public class SourceBuilderExtensionsTests
{
    private static readonly (string Name, string Content)[] _descriptors =
    {
        new("Test", "// Hello There !"),
        new("Test2", "using SourceGeneratorUtils.Tests;"),
        new("NamelessClass", "using SourceGeneratorUtils.Tests;"),
    };

    [Fact]
    public void Register_ShouldPopulateSourceBuilder_WithGeneratedSourceFile()
    {
        var srcFileGenerator = new TestSourceFileGenerator();
        var builder = new SourceBuilder();

        builder.Register(srcFileGenerator, _descriptors[0]);
        True(builder.SourceFiles.TryGetValue("Test", out var generatedFile));
        Equal(_descriptors[0].Content + Environment.NewLine, generatedFile.ToString());
    }

    [Fact]
    public void PopulateWith_ShouldPopulateSourceBuilder_WithGeneratedSourceFiles()
    {
        var srcFileGenerator = new TestSourceFileGenerator();
        var builder = new SourceBuilder();

        builder.PopulateWith(srcFileGenerator, _descriptors);
        VerifySourceBuilder(builder);

        builder = new SourceBuilder();
        builder.PopulateWith(srcFileGenerator, _descriptors.ToList());
        VerifySourceBuilder(builder);
    }

    private static void VerifySourceBuilder(SourceBuilder builder)
    {
        foreach (var (name, content) in _descriptors)
        {
            True(builder.SourceFiles.TryGetValue(name, out var generatedFile));
            Equal(content + Environment.NewLine, generatedFile.ToString());
        }
    }

    private class TestSourceFileGenerator : ISourceFileGenerator<(string Name, string Content)>
    {
        public SourceFile GenerateSource((string Name, string Content) target)
            => new(target.Name, new SourceWriter().WriteLine(target.Content));
    }
}