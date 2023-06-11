﻿using SourceGeneratorUtils.Descriptors;

namespace SourceGeneratorUtils.Tests;

public class CSharpSourceFileGeneratorTests
{
    private static readonly DefaultTypeSpec DefaultTypeSpec = new("Test", "class", "SourceGeneratorUtils.Tests");
    private static readonly SourceFileGenOptions Options = new();
    
    [Fact]
    public void GenerateSource_ShouldUseWriterFactoryIfSpecified()
    {
        const string expected = "// <auto-generated />";
        var generator = new CSharpSourceFileGenerator(Options with
        {
            WriterFactory = static () => new SourceWriter().WriteLine(expected)
        });

        var sourceFile = generator.GenerateSource(DefaultTypeSpec);
        StartsWith(expected, sourceFile.Content.ToString());
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void GenerateSource_ShouldIncludeNullableAnnotation(bool enableNullableAnnotation, bool enableNullableWarnings)
    {
        var expected = $"""
            #nullable {(enableNullableAnnotation ? "enable" : "disable")} annotations
            #nullable {(enableNullableWarnings ? "enable" : "disable")} warnings
            """;

        var generator = new CSharpSourceFileGenerator(Options with
        {
            EnableNullableAnnotations = enableNullableAnnotation,
            EnableNullableWarnings = enableNullableWarnings
        });

        var sourceFile = generator.GenerateSource(DefaultTypeSpec);
        StartsWith(expected, sourceFile.Content.ToString());
    }

    [Fact]
    public void GenerateSource_ShouldWriteUsingDirectives()
    {
        const string expected = """
            using SecondNamespace.WithSubNamespace;
            using SourceGeneratorUtils;
            using MyNamespace1;
            using System;
            """;

        var blockGenerator = new TestBlockGenerator
        {
            ImportedNamespaces = new [] { "MyNamespace1", "SecondNamespace.WithSubNamespace" },
        };

        var generator = new CSharpSourceFileGenerator(Options with
        {
            DefaultUsingDirectives = new[] { "System", "MyNamespace1", "SourceGeneratorUtils" },
            BlockGenerators = new[] { blockGenerator }
        });

        var sourceFile = generator.GenerateSource(DefaultTypeSpec);
        Contains(expected, sourceFile.Content.ToString());
    }

    [Fact]
    public void GenerateSource_ShouldNotWriteUsingDirectives_IfNone()
    {
        var generator = new CSharpSourceFileGenerator(Options);
        var sourceFile = generator.GenerateSource(DefaultTypeSpec);
        DoesNotContain("using", sourceFile.Content.ToString());
    }

    [Theory, InlineData(true), InlineData(false)]
    public void GenerateSource_ShouldDeclareNamespace(bool useFileScopedNamespace)
    {
        const string expectedFileScoped = """
            namespace SourceGeneratorUtils.Tests;

            public class Test
            {
            }
            """;

        const string expectedNamespace = """
            namespace SourceGeneratorUtils.Tests
            {
                public class Test
                {
                }
            }
            """;

        var generator = new CSharpSourceFileGenerator(Options with
        {
            UseFileScopedNamespace = useFileScopedNamespace
        });

        var sourceFile = generator.GenerateSource(DefaultTypeSpec);
        var expected = useFileScopedNamespace ? expectedFileScoped : expectedNamespace;

        EndsWith(expected, sourceFile.Content.ToString());
    }

    [Fact]
    public void GenerateSource_ShouldEncloseTargetInContainingTypes()
    {
        const string expected = """
            namespace SourceGeneratorUtils.Tests
            {
                internal partial class Outer
                {
                    public partial class ContainingType
                    {
                        public class Test
                        {
                        }
                    }
                }
            }
            """;

        var generator = new CSharpSourceFileGenerator(Options);
        var sourceFile = generator.GenerateSource(DefaultTypeSpec with
        {
            ContainingTypes = new ITypeSpec[]
            {
                new DefaultTypeSpec("Outer", "partial class", string.Empty, Accessibility:"internal"),
                new DefaultTypeSpec("ContainingType", "partial class", string.Empty),
            }
        });

        EndsWith(expected, sourceFile.Content.ToString());
    }

    [Theory, InlineData(true), InlineData(false)]
    public void GenerateSource_ShouldWriteAttributes(bool useCombinedAttributes)
    {
        const string expectedCombined = "[MyAttribute, MyAttribute2, AttrWithParam(2)]";
        const string expectedSeparate = """
                [MyAttribute]
                [MyAttribute2]
                [AttrWithParam(2)]
            """;

        var generator = new CSharpSourceFileGenerator(Options with
        {
            UseCombinedAttributes = useCombinedAttributes,
            DefaultAttributes = new[] { "MyAttribute", "MyAttribute2" }
        });

        var sourceFile = generator.GenerateSource(DefaultTypeSpec with
        {
            Attributes = new[] { "MyAttribute", "MyAttribute2", "MyAttribute", "AttrWithParam(2)" }
        });

        Contains(useCombinedAttributes ? expectedCombined : expectedSeparate, sourceFile.Content.ToString());
    }

    [Fact]
    public void GenerateSource_ShouldImplementsInterfacesOnTargetType()
    {
        const string expected = "public class Test : ITest, IReadable, ITest2";
        var blockGenerator = new TestBlockGenerator
        {
            ImplementedInterfaces = new[] { "ITest", "IReadable" },
        };

        var generator = new CSharpSourceFileGenerator(Options with
        {
            DefaultInterfaces = new [] { "ITest", "ITest2"},
            BlockGenerators = new[] { blockGenerator }
        });

        var sourceFile = generator.GenerateSource(DefaultTypeSpec);
        Contains(expected, sourceFile.Content.ToString());
    }

    [Fact]
    public void GenerateSource_ShouldImplementOptionsBaseType_IfNone()
    {
        const string expected = "public class Test : BaseClass";
        var generator = new CSharpSourceFileGenerator(Options with
        {
            DefaultBaseType = "BaseClass"
        });

        var sourceFile = generator.GenerateSource(DefaultTypeSpec);
        Contains(expected, sourceFile.Content.ToString());
    }

    [Fact]
    public void GenerateSource_ShouldImplementTypeSpecBaseType_IfPresent()
    {
        const string expected = """
                public class Test : TestBase
                {
                }
            """;

        var generator = new CSharpSourceFileGenerator(Options);
        var sourceFile = generator.GenerateSource(DefaultTypeSpec with
        {
            BaseTypeName = "TestBase"
        });

        Contains(expected, sourceFile.Content.ToString());
    }

    [Fact]
    public void GenerateSource_ShouldFormatBaseTypeAndInterfacesTogether()
    {
        const string expected = """
                public class Test : TestBase, ITest, IReadable
                {
                }
            """;

        var generator = new CSharpSourceFileGenerator(Options with
        {
            DefaultInterfaces = new[] { "ITest", "IReadable" }
        });

        var sourceFile = generator.GenerateSource(DefaultTypeSpec with
        {
            BaseTypeName = "TestBase"
        });

        Contains(expected, sourceFile.Content.ToString());
    }

    [Fact]
    public void GenerateSource_ShouldWriteSourceBlocks()
    {
        const string expected = """
                public class Test
                {
                }
            """;

        var blockGenerators = new[] { new TestBlockGenerator(), new(), new() };
        var generator = new CSharpSourceFileGenerator(Options with
        {
            BlockGenerators = blockGenerators
        });

        var sourceFile = generator.GenerateSource(DefaultTypeSpec);

        Contains(expected, sourceFile.Content.ToString());
        True(blockGenerators.All(static g => g.GenerateBlockCalled));
    }

    [Fact]
    public void GenerateSource_ShouldAddSpaceBetweenSourceBlocks()
    {
        const string expected = """
                public class Test
                {
                    public void Method1()
                    {
                    }

                    public void Method2()
                    {
                    }
                }
            }
            """;

        var blockGenerators = new[] { new MethodGenerator("Method1"), new("Method2") };
        var generator = new CSharpSourceFileGenerator(Options with
        {
            BlockGenerators = blockGenerators
        });

        var sourceFile = generator.GenerateSource(DefaultTypeSpec);
        EndsWith(expected, sourceFile.Content.ToString());
    }

    private sealed class MethodGenerator : SourceBlockGenerator
    {
        private readonly string _methodName;
        public MethodGenerator(string methodName) => _methodName = methodName;

        public override void GenerateBlock(SourceWriter writer, in SourceWritingContext context)
        {
            writer.WriteLine($"public void {_methodName}()");
            writer.OpenBlock();
            writer.CloseBlock();
        }
    }
    private sealed class TestBlockGenerator : SourceBlockGenerator
    {
        public bool GenerateBlockCalled { get; private set; }
        public string[] ImportedNamespaces { get; set; } = Array.Empty<string>();
        public string[] ImplementedInterfaces { get; set; } = Array.Empty<string>();

        public override IEnumerable<string> GetImplementedInterfaces(in SourceWritingContext context)
            => ImplementedInterfaces;

        public override IEnumerable<string> GetImportedNamespaces(in SourceWritingContext context)
            => ImportedNamespaces;

        public override void GenerateBlock(SourceWriter writer, in SourceWritingContext context)
            => GenerateBlockCalled = true;
    }
}