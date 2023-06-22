using System.Text;

namespace SourceGeneratorUtils.Tests;

public class SourceFileEmitterTests
{
    private static readonly TestGenerationSpec DefaultSpec = new()
    {
        Namespace = "SourceGeneratorUtils.Tests",
        TypeDeclarations = ImmutableEquatableArray.Create("public class TestType", "public partial class ContainingClass1")
    };

    private static readonly SourceCodeEmitter<TestGenerationSpec>[] _sourceCodeEmitters =
    {
        new TestSourceCodeEmitter(),
        new ConfigurableTestSourceCodeEmitter()
    };
    
    [Fact]
    public void TestSourceCodeEmitter_Options_GetShouldThrowIfNotSetup()
        => Throws<InvalidOperationException>(() => new TestSourceCodeEmitter().Options);

    [Theory, InlineData(true), InlineData(false)]
    public void Ctor_ShouldSetupOptionsOnSourceCodeWritersIfNone(bool optionAlreadySetup)
    {
        var sourceCodeEmitters = optionAlreadySetup
            ? new[] { new TestSourceCodeEmitter { Options = new SourceFileEmitterOptions { BlankLinesBetweenDeclarations = 2 } } }
            : new[] { new TestSourceCodeEmitter() };

        var emitter = new TestSourceFileEmitter(SourceFileEmitterOptions.Default, sourceCodeEmitters);

        if (optionAlreadySetup) NotEqual(SourceFileEmitterOptions.Default, emitter.SourceCodeEmitters.First().Options);
        else Equal(SourceFileEmitterOptions.Default, emitter.SourceCodeEmitters.First().Options);
    }

    [Theory, InlineData(true), InlineData(false)]
    public void SourceCodeWriters_Init_ShouldSetupOptionsOnSourceCodeWritersIfNone(bool optionAlreadySetup)
    {
        var sourceCodeEmitters = optionAlreadySetup
            ? new[] { new TestSourceCodeEmitter { Options = new SourceFileEmitterOptions { BlankLinesBetweenDeclarations = 2 } } }
            : new[] { new TestSourceCodeEmitter() };

        var emitter = new TestSourceFileEmitter(SourceFileEmitterOptions.Default) { SourceCodeEmitters = sourceCodeEmitters };

        if (optionAlreadySetup) NotEqual(SourceFileEmitterOptions.Default, emitter.SourceCodeEmitters.First().Options);
        else Equal(SourceFileEmitterOptions.Default, emitter.SourceCodeEmitters.First().Options);
    }

    [Fact]
    public void GetTargetOuterUsingDirectives_ReturnsAllSourceCodeWritersOuterUsingDirectives()
    {
        var sourceCodeEmitters = new[]
        {
            new ConfigurableTestSourceCodeEmitter { OuterUsingDirectives = new [] { "SourceGeneratorUtils"}},
            new ConfigurableTestSourceCodeEmitter { OuterUsingDirectives =new [] { "System"}},
            new ConfigurableTestSourceCodeEmitter { OuterUsingDirectives =new [] { "System.Buffers"}},
        };

        var emitter = new TestSourceFileEmitter(sourceCodeEmitters);

        var expected = sourceCodeEmitters.SelectMany(e => e.OuterUsingDirectives);
        Equal(expected, emitter.GetTargetOuterUsingDirectives(DefaultSpec));
    }

    [Fact]
    public void GetTargetInnerUsingDirectives_ReturnsAllSourceCodeWritersOuterUsingDirectives()
    {
        var sourceCodeEmitters = new[]
        {
            new ConfigurableTestSourceCodeEmitter { InnerUsingDirectives = new [] { "SourceGeneratorUtils"}},
            new ConfigurableTestSourceCodeEmitter { InnerUsingDirectives =new [] { "System"}},
            new ConfigurableTestSourceCodeEmitter { InnerUsingDirectives =new [] { "System.Buffers"}},
        };

        var emitter = new TestSourceFileEmitter(sourceCodeEmitters);

        var expected = sourceCodeEmitters.SelectMany(e => e.InnerUsingDirectives);
        Equal(expected, emitter.GetTargetInnerUsingDirectives(DefaultSpec));
    }

    [Fact]
    public void GetTargetAttributesToApply_ReturnsAllSourceCodeWritersAttributes()
    {
        var sourceCodeEmitters = new[]
        {
            new ConfigurableTestSourceCodeEmitter { AttributesToApply = new [] { "Generate" }},
            new ConfigurableTestSourceCodeEmitter { AttributesToApply = new [] { "DisplayName(Name = \"test\")" }},
            new ConfigurableTestSourceCodeEmitter { AttributesToApply = new [] { "TestAttribute" }},
        };

        var emitter = new TestSourceFileEmitter(sourceCodeEmitters);

        var expected = sourceCodeEmitters.SelectMany(e => e.AttributesToApply);
        Equal(expected, emitter.GetTargetAttributesToApply(DefaultSpec));
    }

    [Fact]
    public void GetTargetInterfacesToImplement_ReturnsAllSourceCodeWritersInterfaces()
    {
        var sourceCodeEmitters = new[]
        {
            new ConfigurableTestSourceCodeEmitter { InterfacesToImplement = new [] { "IInterface" }},
            new ConfigurableTestSourceCodeEmitter { InterfacesToImplement = new [] { "IGeneric<int>" }},
            new ConfigurableTestSourceCodeEmitter { InterfacesToImplement = new [] { "ITestInterface" }},
        };

        var emitter = new TestSourceFileEmitter(sourceCodeEmitters);

        var expected = sourceCodeEmitters.SelectMany(e => e.InterfacesToImplement);
        Equal(expected, emitter.GetTargetInterfacesToImplement(DefaultSpec));
    }

    [Theory, InlineData(-1), InlineData(0), InlineData(1), InlineData(2), InlineData(16)]
    public void EmitTargetSourceCode_ShouldEmitSourceCodeWithConfiguredSpacing(int blankLinesBetweenSourceCodeWriters)
    {
        var emitter = new TestSourceFileEmitter(
            SourceFileEmitterOptions.Default with { BlankLinesBetweenCodeEmitters = blankLinesBetweenSourceCodeWriters },
            _sourceCodeEmitters);

        var writer = new SourceWriter();
        emitter.EmitTargetSourceCode(DefaultSpec, writer);

        string expected = $"""
            // {DefaultSpec.Comment}{EmptyOrNewLines(blankLinesBetweenSourceCodeWriters)}
            // {DefaultSpec.Comment2}
            """;

        StartsWith(expected, writer.ToString());
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
        var sourceCodeEmitters = new[] { new ConfigurableTestSourceCodeEmitter { AttributesToApply = new[] { "Generate" } } };
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
        var sourceCodeEmitters = new[] { new ConfigurableTestSourceCodeEmitter { InterfacesToImplement = new[] { "ITestInterface" } } };
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

        TestGenerationSpec specWithBaseType = new()
        {
            Namespace = "SourceGeneratorUtils.Tests",
            TypeDeclarations = ImmutableEquatableArray.Create("public class TestType : BaseType")
        };

        var output = emitter.CreateSourceWriter(specWithBaseType).ToString();
        DoesNotContain("DefaultBaseType", output);
        Contains(specWithBaseType.TypeDeclarations[0] + ", ITestInterface", output);
    }

    private static string EmptyOrNewLines(int count)
    {
        if (count < 1) return string.Empty;

        var builder = new StringBuilder();
        for (int i = 0; i < count; i++) 
            builder.AppendLine();

        return builder.ToString();
    }

    private sealed record TestGenerationSpec : AbstractTypeGenerationSpec
    {
        public string Comment { get; init; } = "Hello There !";
        public string Comment2 { get; init; } = "Welcome !";
    }

    private sealed record TestSourceCodeEmitter : SourceCodeEmitter<TestGenerationSpec>
    {
        public override void EmitTargetSourceCode(TestGenerationSpec target, SourceWriter writer)
        {
            writer.WriteLine("// " + target.Comment);
        }
    }

    private sealed record ConfigurableTestSourceCodeEmitter : SourceCodeEmitter<TestGenerationSpec>
    {
        public IReadOnlyList<string> OuterUsingDirectives { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> InnerUsingDirectives { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> AttributesToApply { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> InterfacesToImplement { get; init; } = Array.Empty<string>();


        public override void EmitTargetSourceCode(TestGenerationSpec target, SourceWriter writer)
        {
            writer.WriteLine("// " + target.Comment2);
        }

        public override IEnumerable<string> GetOuterUsingDirectives(TestGenerationSpec target) => OuterUsingDirectives;
        public override IEnumerable<string> GetInnerUsingDirectives(TestGenerationSpec target) => InnerUsingDirectives;
        public override IEnumerable<string> GetAttributesToApply(TestGenerationSpec target) => AttributesToApply;
        public override IEnumerable<string> GetInterfacesToImplement(TestGenerationSpec target) => InterfacesToImplement;
    }

    private sealed class TestSourceFileEmitter : SourceFileEmitter<TestGenerationSpec>
    {
        public TestSourceFileEmitter(SourceFileEmitterOptions options, IReadOnlyList<SourceCodeEmitter<TestGenerationSpec>> codeEmitters) : base(options, codeEmitters)
        {
        }

        public TestSourceFileEmitter(IReadOnlyList<SourceCodeEmitter<TestGenerationSpec>> codeEmitters) : base(SourceFileEmitterOptions.Default, codeEmitters)
        {
        }

        public TestSourceFileEmitter(SourceFileEmitterOptions options) : base(options)
        {
        }

        public override string GetFileName(TestGenerationSpec target) => string.Empty;
    }
}