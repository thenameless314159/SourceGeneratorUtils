namespace SourceGeneratorUtils.Tests;

public class SourceWriterTests
{
    const string SeveralLineStringLiteralConstant = """
        Test case with
        several
        newlines.
        Hello, Nameless !
        """;

    [Theory]
    
    [InlineData("Hello, World!")]
    [InlineData("Another test case.")]
    [InlineData(SeveralLineStringLiteralConstant)]
    public void WriteLine_ShouldIndentAllLines(string input)
    {
        var sourceWriter = new SourceWriter();

        // write indented lines
        sourceWriter.Indentation++;
        sourceWriter.WriteLine(input);

        Equal($"    {input.Replace(Environment.NewLine, Environment.NewLine + "    ") + Environment.NewLine}", sourceWriter.ToString());
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