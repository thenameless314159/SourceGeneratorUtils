using System.Reflection;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.Tests;

public class DefaultSourceFileEmitterTests
{
    private static readonly TypeDesc DefaultTypeDesc = TypeDesc.Create
    (
        "TestClass",
        typeKind: TypeKind.Class,
        accessibility: Accessibility.Public,
        @namespace: "SourceGeneratorUtils.Tests"
    );

    [Fact]
    public void GenerateSource_ShouldIncludeTypeDeclarationsAndBody_WithTargetAttributesAndInterfaces()
    {
        var sourceCodeEmitters = new[] 
        { 
            new TestSourceCodeEmitter {
                AttributesToApply = new [] { "Generated" }, 
                InterfacesToImplement = new [] { "ITestInterface" }
            }
        };

        TypeDesc targetTypeDesc = TypeDesc.Create
        (
            "TestType",
            isRecord: true,
            isValueType: true,
            typeKind: TypeKind.Struct,
            accessibility: Accessibility.Public,
            @namespace: "SourceGeneratorUtils.Tests",
            containingTypes: new [] { DefaultTypeDesc }
        );

        var emitter = new DefaultSourceFileEmitter(sourceCodeEmitters);
        var sourceFile = emitter.GenerateSource(DefaultGenerationSpec.CreateFrom(targetTypeDesc));

        const string expected = """
            namespace SourceGeneratorUtils.Tests
            {
                public class TestClass
                {
                    [Generated]
                    public record struct TestType : ITestInterface
                    {
                        // Body for TestType goes here !
                    }
                }
            }
            """;

        EndsWith(expected, sourceFile.Content.ToString());
    }

    private sealed record TestSourceCodeEmitter : DefaultSourceCodeEmitter
    {
        public IReadOnlyList<string> AttributesToApply { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> InterfacesToImplement { get; init; } = Array.Empty<string>();

        public override void EmitTargetSourceCode(DefaultGenerationSpec target, SourceWriter writer)
        {
            writer.WriteLine($"// Body for {target.TargetType.Name} goes here !");
        }

        public override IEnumerable<string> GetAttributesToApply(DefaultGenerationSpec target)
            => AttributesToApply;

        public override IEnumerable<string> GetInterfacesToImplement(DefaultGenerationSpec target)
            => InterfacesToImplement;
    }
}