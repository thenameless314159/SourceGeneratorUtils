namespace SourceGeneratorUtils.Tests;

public class TypeSourceCodeEmitterTests
{
    private static readonly TestTypeGenerationSpec DefaultSpec = new()
    {
        TestNumber = 1,
        Namespace = "SourceGeneratorUtils.Tests",
        TypeDeclarations = ImmutableEquatableArray<string>.Empty
    };

    [Fact]
    public void GetAttributesToApply_ReturnsEmptyEnumerable()
        => Empty(new ThrowTypeSourceCodeEmitter().GetAttributesToApply(DefaultSpec));

    [Fact]
    public void GetInterfacesToImplement_ReturnsEmptyEnumerable()
        => Empty(new ThrowTypeSourceCodeEmitter().GetInterfacesToImplement(DefaultSpec));

    private sealed record TestTypeGenerationSpec : AbstractTypeGenerationSpec
    {
        public required int TestNumber { get; init; }
    }

    private sealed class ThrowTypeSourceCodeEmitter : TypeSourceCodeEmitter<TestTypeGenerationSpec>
    {
        public override void EmitTargetSourceCode(TestTypeGenerationSpec target, SourceWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}