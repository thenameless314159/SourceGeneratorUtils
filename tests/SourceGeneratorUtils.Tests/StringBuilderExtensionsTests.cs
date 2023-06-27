using System.Text;

namespace SourceGeneratorUtils.Tests;

public class StringBuilderExtensionsTests
{
    [Theory]
    [InlineData("Hello There !")]
    [InlineData("Welcome here !")]
    [InlineData("This is a test !")]
    public void AppendSpan_ShouldAppendTheGivenSpan(string value)
    {
        StringBuilder sb = new();
        sb.AppendSpan(value.AsSpan());

        Equal(value, sb.ToString());
    }
}