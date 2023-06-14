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
        foreach (var desc in _descriptors)
        {
            True(builder.SourceFiles.TryGetValue(desc.Name, out var generatedFile));
            Equal("Hello There!" + Environment.NewLine, generatedFile.ToString());
        }
    }

    private class TestSourceFileGenerator : ISourceFileGenerator<DefaultTypeSpec>
    {
        public SourceFileDescriptor GenerateSource(DefaultTypeSpec target)
        {
            return new(target.Name, new SourceWriter().WriteLine("Hello There!"));
        }
    }
}