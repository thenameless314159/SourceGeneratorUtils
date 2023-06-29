namespace SourceGeneratorUtils.SourceGeneration.UnitTests;

public class EmbeddedResourcesStoreTests
{
    [Fact] public void ResourceNameToFileName_ThrowsOnInvalidResourceNamePrefix()
        => Throws<ArgumentException>(() => EmbeddedResourcesStore.ResourceNameToFileName("invalid"));

    [Fact] public void GetEmbeddedResourceContent_ThrowsOnInvalidResourceName()
        => Throws<ArgumentException>(() => EmbeddedResourcesStore.GetEmbeddedResourceContent("invalid"));

    [Fact]
    public void ResourceNameToFileName_ReturnsFileNameWithNoPrefix()
        => Equal("MyResource.g.cs", EmbeddedResourcesStore.ResourceNameToFileName("SourceGeneratorUtils.SourceGeneration.MyResource.cs"));

    [Fact]
    public void GetEmbeddedResourceContent_ShouldReadResourceContentForAvailableResources()
    {
        foreach (string resourceName in EmbeddedResourcesStore.FileNamesByResourceName.Keys)
        {
            string resourceContent = EmbeddedResourcesStore.GetEmbeddedResourceContent(resourceName);
            NotSame(EmbeddedResourcesStore.GetEmbeddedResourceContent(resourceName), resourceContent);
            False(string.IsNullOrWhiteSpace(resourceContent));
        }
    }

    [Fact]
    public void GetEmbeddedResourceContent_ShouldCacheContentIfSpecified()
    {
        foreach (string resourceName in EmbeddedResourcesStore.FileNamesByResourceName.Keys)
        {
            string resourceContent = EmbeddedResourcesStore.GetEmbeddedResourceContent(resourceName, cacheResourceContent: true);
            True(EmbeddedResourcesStore.CachedEmbeddedResources.TryGetValue(resourceName, out var cachedContent));
            Same(resourceContent, cachedContent);
        }
    }

    [Theory]
    [MemberData(nameof(GetEmbeddedResourceNames))]
    public void FileNamesByResourceName_ShouldMakeGeneratedFileName(string resourceName)
    {
        const string fileExtension = ".cs", genExtension = ".g.cs";
        True(EmbeddedResourcesStore.FileNamesByResourceName.TryGetValue(resourceName, out string? fileName));
        DoesNotContain(WellKnownStrings.InfrastructureNamespace, fileName);
        DoesNotContain(WellKnownStrings.LocalAssemblyNamespace, fileName);
        DoesNotContain(WellKnownStrings.DescriptorsNamespace, fileName);
        DoesNotContain(WellKnownStrings.ExtensionsNamespace, fileName);
        DoesNotContain(WellKnownStrings.PolyfillsNamespace, fileName);
        Contains(fileExtension, resourceName);
        Contains(genExtension, fileName);
    }

    public static IEnumerable<object[]> GetEmbeddedResourceNames()
        => EmbeddedResourcesStore.FileNamesByResourceName.Keys.Select(static rn => new[] { rn });
}