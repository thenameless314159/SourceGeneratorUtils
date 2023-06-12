using System.Text;

namespace SourceGeneratorUtils.Tests;

public class SourceWriterTests
{
    [Fact] public void Ctor_ThrowsIfIndentationCharIsNotWhiteSpace() 
        => Throws<ArgumentOutOfRangeException>(static () => new SourceWriter('a', 4));

    [Theory, InlineData(0), InlineData(-1)]
    public void Ctor_ThrowsIfCharsPerIndentationIsLessThanOne(int indentation)
        => Throws<ArgumentOutOfRangeException>(() => new SourceWriter(' ', indentation));

    [Fact] public void Indentation_Setter_ThrowsIfIndentationIsLessThanOne() 
        => Throws<ArgumentOutOfRangeException>(() => new SourceWriter { Indentation = -1 });

    const string SeveralLinesStringLiteralConstant = """
        Test case with
        several
        newlines.
        Hello, Nameless !
        """;

    [Theory]
    [InlineData("Hello, World!")]
    [InlineData("Another test case.")]
    [InlineData(SeveralLinesStringLiteralConstant)]
    public void WriteLine_ShouldIndentAllLines(string input)
    {
        var sourceWriter = new SourceWriter();

        // write indented lines
        sourceWriter.Indentation++;
        sourceWriter.WriteLine(input);

        Equal($"    {input.Replace(Environment.NewLine, Environment.NewLine + "    ") + Environment.NewLine}", sourceWriter.ToString());
    }

    [Fact]
    public void WriteLine_EmptyString_ShouldAppendIndentedEmptyNewLine()
    {
        var sourceWriter = new SourceWriter();

        sourceWriter.Indentation++;
        sourceWriter.WriteLine(string.Empty);

        Equal("    " + Environment.NewLine, sourceWriter.ToString());
    }

    [Fact]
    public void WriteLine_NoParams_ShouldAppendEmptyNewLine()
    {
        var sourceWriter = new SourceWriter();
        sourceWriter.WriteLine();

        Equal(Environment.NewLine, sourceWriter.ToString());
    }

    [Fact]
    public void WriteLine_Char_ShouldIndentAndAppendCharWithNewLine()
    {
        var sourceWriter = new SourceWriter();
        sourceWriter.Indentation++;
        sourceWriter.WriteLine('c');

        Equal("    c" + Environment.NewLine, sourceWriter.ToString());
    }

    [Fact]
    public void ToSourceText_ReturnsUtf8EncodedSourceText()
    {
        var writer = new SourceWriter();
        writer.WriteLine("using System;");
        writer.WriteLine();
        writer.WriteLine("namespace MyNamespace;");
        writer.WriteLine();
        writer.WriteLine("public class MyClass");
        writer.OpenBlock();
        writer.WriteLine("public void MyMethod()");
        writer.OpenBlock();
        writer.WriteLine("// TODO: Add implementation");
        writer.CloseBlock();
        writer.CloseBlock(); // MyClass

        // require to import Microsoft.CodeAnalysis.CSharp in the target assembly
        var sourceText = writer.ToSourceText();
        const string expectedOutput = """
            using System;

            namespace MyNamespace;
            
            public class MyClass
            {
                public void MyMethod()
                {
                    // TODO: Add implementation
                }
            }
            """;

        Equal(writer.Length, sourceText.Length);
        Equal(Encoding.UTF8, sourceText.Encoding);
        Equal(expectedOutput, sourceText.ToString());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToString_ShouldMakeNewStringIfCached(bool useCachedToString)
    {
        var sourceWriter = new SourceWriter(cacheToString: useCachedToString);
        sourceWriter.WriteLine("Hello World!");
        
        if (useCachedToString)
            Same(sourceWriter.ToString(), sourceWriter.ToString());
        else
            NotSame(sourceWriter.ToString(), sourceWriter.ToString());
    }

    [Fact]
    public void OpenCloseAllBlocks_ShouldAppendNotIndentedBraceWithoutNewLine()
    {
        var sourceWriter = new SourceWriter();

        sourceWriter.OpenBlock();
        sourceWriter.WriteLine("Hello, World!");
        sourceWriter.CloseAllBlocks();

        const string expectedOutput ="""
            {
                Hello, World!
            }
            """;
       
        Equal(expectedOutput, sourceWriter.ToString());
    }
}