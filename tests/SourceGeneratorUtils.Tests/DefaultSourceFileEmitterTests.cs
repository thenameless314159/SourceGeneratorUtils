using System.Reflection;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.Tests;

public class DefaultSourceFileEmitterTests
{
    private static readonly TypeDesc DefaultTypeDesc = new()
    {
        Accessibility = Accessibility.Public,
        Namespace = "SourceGeneratorUtils.Tests",
        Name = "TestClass",
        TypeKind = TypeKind.Class,
        IsRecord = false,
        TypeModifier = null,
        IsValueType = false,
        SpecialType = SpecialType.None
    };

    [Fact]
    public void GenerateSource_ShouldIncludeTypeDeclarationsAndBody_WithTargetAttributesAndInterfaces()
    {
        var sourceCodeEmitters = new[] { new TestSourceCodeEmitter 
            { AttributesToApply = new [] { "Generated" }, InterfacesToImplement = new [] { "ITestInterface" } }
        };

        TypeDesc targetTypeDesc = new() {
            TypeKind = TypeKind.Class, IsRecord = false, TypeModifier = null, IsValueType = false, SpecialType = SpecialType.None,
            ContainingTypes = ImmutableEquatableArray.Create(DefaultTypeDesc),
            Accessibility = Accessibility.Public,
            Namespace = "SourceGeneratorUtils.Tests",
            Name = "TestType",
        };

        var emitter = new DefaultSourceFileEmitter(sourceCodeEmitters);
        var sourceFile = emitter.GenerateSource(DefaultGenerationSpec.CreateFrom(targetTypeDesc));

        const string expected = """
            namespace SourceGeneratorUtils.Tests
            {
                public class TestClass
                {
                    [Generated]
                    public class TestType : ITestInterface
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