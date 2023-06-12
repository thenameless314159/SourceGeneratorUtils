namespace SourceGeneratorUtils.Tests;

public class SourceBlockGeneratorTests
{
    private static readonly SourceFileGenOptions _options = new();

    [Fact]
    public void GenerateBlock_ThrowsIfTargetIsWrongType()
    {
        var target = new TestTypeSpec("Test", string.Empty, "SourceGeneratorUtils.Tests");
        var generator = new TestSourceBlockGenerator();

        Throws<ArgumentOutOfRangeException>(() 
            => generator.GenerateBlock(new SourceWriter(), SourceWritingContext.CreateFor(target, _options)));
    }

    [Fact]
    public void GenerateBlock_ThrowsIfTypeDescriptorsIsNotTypeDescriptorStore()
    {
        var target = new DefaultTypeSpec("Test", string.Empty, "SourceGeneratorUtils.Tests");
        var wrongStore = new Dictionary<string, ITypeSpec>();
        var generator = new TestSourceBlockGenerator();

        Throws<ArgumentOutOfRangeException>(() 
            => generator.GenerateBlock(new SourceWriter(), SourceWritingContext.CreateFor(target, _options, wrongStore)));
    }

    [Fact]
    public void GenerateBlock_CallsTypedGenerateBlock()
    {
        var target = new DefaultTypeSpec("Test", string.Empty, "SourceGeneratorUtils.Tests");
        var generator = new TestSourceBlockGenerator();

        generator.GenerateBlock(new SourceWriter(), SourceWritingContext.CreateFor(target, _options)); 
        True(generator.GenerateBlockCalled);
    }

    private record TestTypeSpec
        (string Name, string Type, string Namespace, string? BaseTypeName = null, string? Accessibility = null) : ITypeSpec
    {
        public IList<string> Attributes { get; init; } = Array.Empty<string>();
        public IList<ITypeSpec> ContainingTypes { get; init; } = Array.Empty<ITypeSpec>();
        public IList<ITypeSpec> BaseTypesAndThis { get; init; } = Array.Empty<ITypeSpec>();
    }

    private class TestSourceBlockGenerator : SourceBlockGenerator<DefaultTypeSpec>
    {
        public bool GenerateBlockCalled { get; private set; }

        protected override void GenerateBlock(SourceWriter writer, in TypedSourceWritingContext context)
            => GenerateBlockCalled = true;
    }
}