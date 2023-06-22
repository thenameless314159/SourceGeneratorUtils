namespace SourceGeneratorUtils.Tests;

public class SourceCodeEmitterTests
{
    private static readonly TestTypeGenerationSpec DefaultSpec = new()
    {
        TestNumber = 1,
        Namespace = "SourceGeneratorUtils.Tests",
        TypeDeclarations = ImmutableEquatableArray<string>.Empty
    };

    [Fact]
    public void Ctor_Default_CreateInstanceWithNullOptions()
    {
        var emitter = new ThrowTypeSourceCodeEmitter();

        Throws<InvalidOperationException>(() => emitter.Options);
    }

    [Fact]
    public void Ctor_Options_CreateInstanceWithSpecifiedOptions()
    {
        var emitter = new ThrowTypeSourceCodeEmitter(SourceFileEmitterOptions.Default);
        Same(SourceFileEmitterOptions.Default, emitter.Options);
    }

    [Fact] public void GetOuterUsingDirectives_ReturnsEmptyEnumerable()
        => Empty(new ThrowTypeSourceCodeEmitter().GetOuterUsingDirectives(DefaultSpec));

    [Fact] public void GetInnerUsingDirectives_ReturnsEmptyEnumerable()
        => Empty(new ThrowTypeSourceCodeEmitter().GetInnerUsingDirectives(DefaultSpec));

    [Fact] public void GetAttributesToApply_ReturnsEmptyEnumerable()
        => Empty(new ThrowTypeSourceCodeEmitter().GetAttributesToApply(DefaultSpec));

    [Fact] public void GetInterfacesToImplement_ReturnsEmptyEnumerable()
        => Empty(new ThrowTypeSourceCodeEmitter().GetInterfacesToImplement(DefaultSpec));

    private sealed record TestTypeGenerationSpec : AbstractTypeGenerationSpec
    {
        public required int TestNumber { get; init; }
    }

    private sealed record ThrowTypeSourceCodeEmitter : SourceCodeEmitter<TestTypeGenerationSpec>
    {
        public ThrowTypeSourceCodeEmitter(SourceFileEmitterOptions options) : base(options)
        {
        }

        public ThrowTypeSourceCodeEmitter()
        {
        }

        public override void EmitTargetSourceCode(TestTypeGenerationSpec target, SourceWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}