namespace SourceGeneratorUtils.SourceGeneration.IntegrationTests;

public static class SourceGenerationTests
{
    private static readonly Type[] _types = TestLibrary.Assembly.GetTypes();

    [Fact] public static void ConsumingAssembly_ShouldExcludeDefaultImplementations() 
        => False(
            _types.Any(static t => t.FullName == "SourceGeneratorUtils.DefaultGenerationSpec") &&
            _types.Any(static t => t.FullName == "SourceGeneratorUtils.DefaultSourceCodeEmitter") &&
            _types.Any(static t => t.FullName == "SourceGeneratorUtils.DefaultSourceFileEmitter"));

}