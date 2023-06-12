namespace SourceGeneratorUtils.Tests;

public class StringHelpersTests
{
    [Theory]
    [InlineData("word", "Word")]
    [InlineData("Hello world", "Hello world")]
    [InlineData("HelLo world", "HelLo world")]
    public void FirstCharToUpperInvariant_ShouldCapitalizeInput(string input, string expectedOutput)
        => Equal(expectedOutput, input.FirstCharToUpperInvariant());

    [Theory, InlineData(null), InlineData("    ")]
    public void FirstCharToUpperInvariant_ShouldThrowsIfInputIsNullOrWhitespace(string? input)
        => Throws<ArgumentNullException>(() => input!.FirstCharToUpperInvariant());

    [Theory]
    [InlineData("hello world", "hello world")]
    [InlineData("Hello world", "hello world")]
    [InlineData("HelLo world", "helLo world")]
    public void FirstCharToLowerInvariant_ShouldUnCapitalizeInput(string input, string expectedOutput)
        => Equal(expectedOutput, input.FirstCharToLowerInvariant());
    
    [Theory, InlineData(null), InlineData("    ")]
    public void FirstCharToLowerInvariant_ShouldThrowsIfInputIsNullOrWhitespace(string? input)
        => Throws<ArgumentNullException>(() => input!.FirstCharToLowerInvariant());

    [Theory]
    [InlineData("System.Text.Json")]
    [InlineData("  System.Net.Http")]
    [InlineData("System.Net.Http")]
    [InlineData("System.Linq.Expressions")]
    [InlineData("System.Linq   ")]
    public void MakeUsingDirective_ReturnsValidUsingDirective(string input)
    {
        var result = StringHelpers.MakeUsingDirective(input);

        StartsWith("using ", result);
        Contains(input.Trim(), result);
        EndsWith(";", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MakeUsingDirective_ThrowsIfInputIsNullOrWhiteSpace(string input)
        => Throws<ArgumentNullException>(() => StringHelpers.MakeUsingDirective(input));

    [Theory]
    [InlineData("using System.Net.Http;")]
    [InlineData("using System.Text.Json")]
    [InlineData("System.Linq.Expressions")]
    [InlineData("System.Linq.Expressions;")]
    public void MakeUsingDirective_Does_Not_Add_Duplicate_Semicolon(string input)
    {
        var result = StringHelpers.MakeUsingDirective(input);
        
        StartsWith("using ", result);
        EndsWith(";", result);
        Contains(input, result);
        DoesNotContain(";;", result);
    }
}