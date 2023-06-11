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
    public void FirstCharToUpperInvariant_ShouldThrowsIfInputNullOrWhitespace(string? input)
        => Throws<ArgumentNullException>(() => input!.FirstCharToUpperInvariant());

    [Theory]
    [InlineData("hello world", "hello world")]
    [InlineData("Hello world", "hello world")]
    [InlineData("HelLo world", "helLo world")]
    public void FirstCharToLowerInvariant_ShouldUnCapitalizeInput(string input, string expectedOutput)
        => Equal(expectedOutput, input.FirstCharToLowerInvariant());
    
    [Theory, InlineData(null), InlineData("    ")]
    public void FirstCharToLowerInvariant_ShouldThrowsIfInputNullOrWhitespace(string? input)
        => Throws<ArgumentNullException>(() => input!.FirstCharToLowerInvariant());
}