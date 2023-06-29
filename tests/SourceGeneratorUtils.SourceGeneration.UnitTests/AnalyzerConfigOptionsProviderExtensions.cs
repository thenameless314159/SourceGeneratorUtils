namespace SourceGeneratorUtils.SourceGeneration.UnitTests;

public class AnalyzerConfigOptionsProviderExtensions
{
    [Theory, InlineData("build_property.test", true), InlineData("build_property.test2", false)]
    public void TryGetGlobalOptionsValue_Bool_ReturnsConfiguredValue(string propertyName, bool expectedValue)
    {
        var optionsProvider = new TestAnalyzerConfigOptionsProvider((propertyName, expectedValue.ToString()));

        True(optionsProvider.TryGetGlobalOptionsValue(propertyName, out bool actualValue));
        Equal(expectedValue, actualValue);
    }
    
    [Fact]
    public void TryGetGlobalOptionsValue_Bool_ReturnsFalse_WhenPropertyIsNotConfigured()
    {
        var optionsProvider = new TestAnalyzerConfigOptionsProvider();

        False(optionsProvider.TryGetGlobalOptionsValue("build_property.nonexistent", out bool actual));
        False(actual);
    }

    [Theory]
    [InlineData("build_property.single", "singleValue")]
    [InlineData("build_property.comma", "value1,value2,value3")]
    [InlineData("build_property.semicolon", "value1;value2;value3")]
    public void TryGetGlobalOptionsValue_StringArray_ReturnsConfiguredValue(string propertyName, string expectedValue)
    {
        var optionsProvider = new TestAnalyzerConfigOptionsProvider((propertyName, expectedValue));

        True(optionsProvider.TryGetGlobalOptionsValue(propertyName, out ImmutableEquatableArray<string>? actual));

        var expected = expectedValue.Split(new[] { ',', ';' }, StringSplitOptions.None);
        Equal(expected, actual);
    }

    [Fact]
    public void TryGetGlobalOptionsValue_StringArray_ReturnsNull_WhenPropertyIsNotConfigured()
    {
        var optionsProvider = new TestAnalyzerConfigOptionsProvider();

        False(optionsProvider.TryGetGlobalOptionsValue("build_property.nonexistent", out ImmutableEquatableArray<string>? actual));
        Null(actual);
    }
}