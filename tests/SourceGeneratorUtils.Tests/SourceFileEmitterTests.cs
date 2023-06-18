using System.Text;

namespace SourceGeneratorUtils.Tests;

public class SourceFileEmitterTests
{
    private static readonly TestSourceGenerationSpec DefaultSpec = new() { Namespace = "SourceGeneratorUtils.Tests" };
    private static readonly SourceCodeEmitter<TestSourceGenerationSpec>[] _sourceCodeEmitters =
    {
        new TestSourceCodeEmitter(),
        new SourceCodeEmitterWithUsingDirectives()
    };
    
    [Fact]
    public void TestSourceCodeEmitter_Options_GetShouldThrowIfNotSetup()
        => Throws<InvalidOperationException>(() => new TestSourceCodeEmitter().Options);

    [Fact]
    public void GetTargetOuterUsingDirectives_ReturnsAllSourceCodeWritersOuterUsingDirectives()
    {
        var sourceCodeEmitters = new[]
        {
            new SourceCodeEmitterWithUsingDirectives { OuterUsingDirectives = new [] { "SourceGeneratorUtils"}},
            new SourceCodeEmitterWithUsingDirectives { OuterUsingDirectives =new [] { "System"}},
            new SourceCodeEmitterWithUsingDirectives { OuterUsingDirectives =new [] { "System.Buffers"}},
        };

        var emitter = new TestSourceFileEmitter { SourceCodeEmitters = sourceCodeEmitters };

        var expected = sourceCodeEmitters.SelectMany(e => e.OuterUsingDirectives);
        Equal(expected, emitter.GetTargetOuterUsingDirectives(DefaultSpec));
    }

    [Fact]
    public void GetTargetInnerUsingDirectives_ReturnsAllSourceCodeWritersOuterUsingDirectives()
    {
        var sourceCodeEmitters = new[]
        {
            new SourceCodeEmitterWithUsingDirectives { InnerUsingDirectives = new [] { "SourceGeneratorUtils"}},
            new SourceCodeEmitterWithUsingDirectives { InnerUsingDirectives =new [] { "System"}},
            new SourceCodeEmitterWithUsingDirectives { InnerUsingDirectives =new [] { "System.Buffers"}},
        };

        var emitter = new TestSourceFileEmitter { SourceCodeEmitters = sourceCodeEmitters };

        var expected = sourceCodeEmitters.SelectMany(e => e.InnerUsingDirectives);
        Equal(expected, emitter.GetTargetInnerUsingDirectives(DefaultSpec));
    }

    [Theory, InlineData(true), InlineData(false)]
    public void GenerateSource_ShouldSetupOptionsOnSourceCodeWritersIfNone(bool optionAlreadySetup)
    {
        var sourceCodeEmitters = optionAlreadySetup 
            ? new[] { new TestSourceCodeEmitter { Options = new SourceFileEmitterOptions { SpacesBetweenDeclarations = 2 } } }
            : new[] { new TestSourceCodeEmitter() };

        var emitter = new TestSourceFileEmitter { SourceCodeEmitters = sourceCodeEmitters };

        False(emitter._areOptionsSetup);
        _ = emitter.GenerateSource(DefaultSpec);

        if (optionAlreadySetup) NotEqual(SourceFileEmitterOptions.Default, sourceCodeEmitters[0].Options);
        else Equal(SourceFileEmitterOptions.Default, sourceCodeEmitters[0].Options);

        True(emitter._areOptionsSetup);
    }

    [Theory, InlineData(-1), InlineData(0), InlineData(1), InlineData(2), InlineData(16)]
    public void EmitTargetSourceCode_ShouldEmitSourceCodeWithConfiguredSpacing(int spacesBetweenDeclarations)
    {
        var emitter = new TestSourceFileEmitter(new SourceFileEmitterOptions
        {
            SpacesBetweenDeclarations = spacesBetweenDeclarations }) {
            SourceCodeEmitters = _sourceCodeEmitters
        };

        var writer = new SourceWriter();
        emitter.EmitTargetSourceCode(DefaultSpec, writer);

        string expected = $"""
            // {DefaultSpec.Comment}{EmptyOrNewLines(spacesBetweenDeclarations)}
            // {DefaultSpec.Comment2}
            """;

        StartsWith(expected, writer.ToString());
    }

    private static string EmptyOrNewLines(int count)
    {
        if (count < 1) return string.Empty;

        var builder = new StringBuilder();
        for (int i = 0; i < count; i++) 
            builder.AppendLine();

        return builder.ToString();
    }

    private sealed record TestSourceGenerationSpec : AbstractGenerationSpec
    {
        public string Comment { get; init; } = "Hello There !";
        public string Comment2 { get; init; } = "Welcome !";
    }

    private sealed class TestSourceCodeEmitter : SourceCodeEmitter<TestSourceGenerationSpec>
    {
        public override void EmitTargetSourceCode(TestSourceGenerationSpec target, SourceWriter writer)
        {
            writer.WriteLine("// " + target.Comment);
        }
    }

    private sealed class SourceCodeEmitterWithUsingDirectives : SourceCodeEmitter<TestSourceGenerationSpec>
    {
        public IReadOnlyList<string> OuterUsingDirectives { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> InnerUsingDirectives { get; init; } = Array.Empty<string>();

        public override void EmitTargetSourceCode(TestSourceGenerationSpec target, SourceWriter writer)
        {
            writer.WriteLine("// " + target.Comment2);
        }

        public override IEnumerable<string> GetOuterUsingDirectives(TestSourceGenerationSpec target) => OuterUsingDirectives;
        public override IEnumerable<string> GetInnerUsingDirectives(TestSourceGenerationSpec target) => InnerUsingDirectives;
    }

    private sealed class TestSourceFileEmitter : SourceFileEmitter<TestSourceGenerationSpec>
    {
        public IReadOnlyList<SourceCodeEmitter<TestSourceGenerationSpec>> SourceCodeEmitters { get; init; } 
            = Array.Empty<SourceCodeEmitter<TestSourceGenerationSpec>>();

        public TestSourceFileEmitter(SourceFileEmitterOptions options) : base(options)
        {
        }

        public TestSourceFileEmitter() : base(SourceFileEmitterOptions.Default)
        {
        }

        public override string GetFileName(TestSourceGenerationSpec target) => string.Empty;

        public override IEnumerable<SourceCodeEmitter<TestSourceGenerationSpec>> GetSourceCodeEmitters()
            => SourceCodeEmitters;
    }
}