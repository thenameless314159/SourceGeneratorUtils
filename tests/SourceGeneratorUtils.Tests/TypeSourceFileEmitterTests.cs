namespace SourceGeneratorUtils.Tests;

public class TypeSourceFileEmitterTests
{
    /*private static readonly TestTypeGenerationSpec DefaultSpec = new()
    {
        Namespace = "SourceGeneratorUtils.Tests",
        TypeDeclarations = ImmutableEquatableArray.Create("public class TestType", "public partial class ContainingClass1")
    };

    [Fact] public void GetTargetAttributesToApply_ReturnsEmptyEnumerable()
        => Empty(new TestSourceFileEmitter().GetTargetAttributesToApply(DefaultSpec));

    [Fact] public void GetTargetInterfacesToImplement_ReturnsEmptyEnumerable()
        => Empty(new TestSourceFileEmitter().GetTargetInterfacesToImplement(DefaultSpec));

    [Fact]
    public void GetTargetAttributesToApply_ReturnsAllSourceCodeWritersAttributes()
    {
        var sourceCodeEmitters = new[]
        {
            new TestSourceCodeEmitter { AttributesToApply = new [] { "Generate" }},
            new TestSourceCodeEmitter { AttributesToApply = new [] { "DisplayName(Name = \"test\")" }},
            new TestSourceCodeEmitter { AttributesToApply = new [] { "TestAttribute" }},
        };

        var emitter = new TestSourceFileEmitter { SourceCodeEmitters = sourceCodeEmitters };

        var expected = sourceCodeEmitters.SelectMany(e => e.AttributesToApply);
        Equal(expected, emitter.GetTargetAttributesToApply(DefaultSpec));
    }

    [Fact]
    public void GetTargetInterfacesToImplement_ReturnsAllSourceCodeWritersInterfaces()
    {
        var sourceCodeEmitters = new[]
        {
            new TestSourceCodeEmitter { InterfacesToImplement = new [] { "IInterface" }},
            new TestSourceCodeEmitter { InterfacesToImplement = new [] { "IGeneric<int>" }},
            new TestSourceCodeEmitter { InterfacesToImplement = new [] { "ITestInterface" }},
        };

        var emitter = new TestSourceFileEmitter { SourceCodeEmitters = sourceCodeEmitters };

        var expected = sourceCodeEmitters.SelectMany(e => e.InterfacesToImplement);
        Equal(expected, emitter.GetTargetInterfacesToImplement(DefaultSpec));
    }

    [Fact]
    public void CreateSourceWriter_ShouldIncludeConfiguredGeneratedCodeAttribute()
    {
        var localAssemblyName = typeof(DefaultSourceFileEmitterTests).Assembly.GetName();
        var options = SourceFileEmitterOptions.Default with { AssemblyName = localAssemblyName };

        var emitter = new TestSourceFileEmitter(options);
        var writer = emitter.CreateSourceWriter(DefaultSpec);

        string expected = $"""[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{options.AssemblyName.Name}", "{options.AssemblyName.Version}")]""";
        Contains(expected, writer.ToString());
    }

    [Theory, InlineData(true), InlineData(false)]
    public void CreateSourceWriter_ShouldIncludeConfiguredDefaultAttributes(bool useCombinedAttributeDeclaration)
    {
        var sourceCodeEmitters = new[] { new TestSourceCodeEmitter { AttributesToApply = new[] { "Generate" } } };
        var options = new SourceFileEmitterOptions
        {
            DefaultAttributes = new[] { "TestAttribute" },
            UseCombinedAttributeDeclaration = useCombinedAttributeDeclaration
        };

        var emitter = new TestSourceFileEmitter(options) { SourceCodeEmitters = sourceCodeEmitters };
        var output = emitter.CreateSourceWriter(DefaultSpec).ToString();

        if (useCombinedAttributeDeclaration)
            Contains("[TestAttribute, Generate]", output);
        else
        {
            Contains("[Generate]", output);
            Contains("[TestAttribute]", output);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("BaseType")]
    public void CreateSourceWriter_ShouldIncludeConfiguredDefaultInterfaces(string? defaultBaseType)
    {
        var sourceCodeEmitters = new[] { new TestSourceCodeEmitter { InterfacesToImplement = new[] { "ITestInterface" } } };
        var options = new SourceFileEmitterOptions
        {
            DefaultBaseType = defaultBaseType,
            DefaultInterfaces = new[] { "IDefaultInterface" },
        };

        var emitter = new TestSourceFileEmitter(options) { SourceCodeEmitters = sourceCodeEmitters };
        var output = emitter.CreateSourceWriter(DefaultSpec).ToString();

        if (defaultBaseType is not null)
            Contains("public class TestType : BaseType, IDefaultInterface, ITestInterface", output);
        else
            Contains("public class TestType : IDefaultInterface, ITestInterface", output);
    }

    [Fact]
    public void CreateSourceWriter_ShouldNotIncludeBaseTypeIfAlreadyPresent()
    {
        var options = new SourceFileEmitterOptions
        {
            DefaultBaseType = "DefaultBaseType",
            DefaultInterfaces = new[] { "ITestInterface" },
        };

        var emitter = new TestSourceFileEmitter(options);

        TestTypeGenerationSpec specWithBaseType = new()
        {
            Namespace = "SourceGeneratorUtils.Tests",
            TypeDeclarations = ImmutableEquatableArray.Create("public class TestType : BaseType")
        };

        var output = emitter.CreateSourceWriter(specWithBaseType).ToString();
        DoesNotContain("DefaultBaseType", output);
        Contains(specWithBaseType.TypeDeclarations[0] + ", ITestInterface", output);
    }

    private sealed record TestTypeGenerationSpec : AbstractTypeGenerationSpec
    {
        public string Comment { get; init; } = "Hello There !";
    }

    private sealed class TestSourceCodeEmitter : TypeSourceCodeEmitter<TestTypeGenerationSpec>
    {
        

        public override void EmitTargetSourceCode(TestTypeGenerationSpec target, SourceWriter writer)
        {
            writer.WriteLine($"// {target.Comment}");
        }
        public IReadOnlyList<string> AttributesToApply { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> InterfacesToImplement { get; init; } = Array.Empty<string>();

        public override IEnumerable<string> GetAttributesToApply(TestTypeGenerationSpec target)
            => AttributesToApply;

        public override IEnumerable<string> GetInterfacesToImplement(TestTypeGenerationSpec target)
            => InterfacesToImplement;
    }

    private sealed class TestSourceFileEmitter : TypeSourceFileEmitter<TestTypeGenerationSpec>
    {
        public IReadOnlyList<TypeSourceCodeEmitter<TestTypeGenerationSpec>> SourceCodeEmitters { get; init; }
            = Array.Empty<TypeSourceCodeEmitter<TestTypeGenerationSpec>>();


        public TestSourceFileEmitter(SourceFileEmitterOptions options) : base(options)
        {
        }

        public TestSourceFileEmitter() : this(SourceFileEmitterOptions.Default)
        {
        }

        public override string GetFileName(TestTypeGenerationSpec target) => string.Empty;

        public override IEnumerable<TypeSourceCodeEmitter<TestTypeGenerationSpec>> GetTypeSourceCodeEmitters()
            => SourceCodeEmitters;
    }*/
}