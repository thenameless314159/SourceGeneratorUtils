namespace SourceGeneratorUtils.SourceGeneration.UnitTests;

public class EmbeddedResourcesStoreTests
{
    [Fact]
    public void GetEmbeddedResourceContent_ShouldReadResourceContentForAvailableResources()
    {
        foreach (string resourceName in EmbeddedResourcesStore.FileNamesByResourceName.Keys)
        {
            string resourceContent = EmbeddedResourcesStore.GetEmbeddedResourceContent(resourceName);
            False(string.IsNullOrWhiteSpace(resourceContent));
        }
    }

    [Fact]
    public void GetEmbeddedResourceContent_ShouldCacheContentIfSpecified()
    {
        foreach (string resourceName in EmbeddedResourcesStore.FileNamesByResourceName.Keys)
        {
            // Ensure the cache is empty before the test
            False(EmbeddedResourcesStore.CachedEmbeddedResources.ContainsKey(resourceName));

            string resourceContent = EmbeddedResourcesStore.GetEmbeddedResourceContent(resourceName, cacheResourceContent: true);
            True(EmbeddedResourcesStore.CachedEmbeddedResources.TryGetValue(resourceName, out var cachedContent));
            Equal(resourceContent, cachedContent);
        }
    }

    [Fact]
    public void FileNamesByResourceName_ShouldMakeGeneratedFileName()
    {
        const string fileExtension = ".cs", genExtension = ".g.cs";

        foreach (var (resourceName, fileName) in EmbeddedResourcesStore.FileNamesByResourceName)
        {
            DoesNotContain(WellKnownStrings.DescriptorsNamespace, fileName);
            DoesNotContain(WellKnownStrings.ExtensionsNamespace, fileName);
            DoesNotContain(WellKnownStrings.InfrastructureNamespace, fileName);
            DoesNotContain(WellKnownStrings.LocalAssemblyNamespace, fileName);
            Contains(fileExtension, resourceName);
            Contains(genExtension, fileName);
        }
    }
}