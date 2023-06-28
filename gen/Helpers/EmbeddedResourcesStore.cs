using static SourceGeneratorUtils.SourceGeneration.WellKnownStrings;
using System.Collections.Concurrent;
using System.Text;

namespace SourceGeneratorUtils.SourceGeneration;

internal static class EmbeddedResourcesStore
{
    public static readonly IReadOnlyDictionary<string, string> FileNamesByResourceName = SourceGeneratorUtilsGenerator.Assembly
        .GetManifestResourceNames()
        .ToDictionary(
            keySelector: static r => r,
            elementSelector: ResourceNameToFileName, 
            comparer: StringComparer.InvariantCulture);

    public static IReadOnlyDictionary<string, string> CachedEmbeddedResources => _cachedEmbeddedResources;

    private static readonly ConcurrentDictionary<string, string> _cachedEmbeddedResources = new(StringComparer.InvariantCulture);

    public static string GetEmbeddedResourceContent(string name, bool cacheResourceContent = false)
    {
        if (!FileNamesByResourceName.ContainsKey(name))
            throw new ArgumentException($"The resource with name '{name}' does not exist.", nameof(name));

        string resourceContent = cacheResourceContent
            ? _cachedEmbeddedResources.GetOrAdd(name, readEmbeddedResource)
            : readEmbeddedResource(name);

        return resourceContent;
        
        static string readEmbeddedResource(string name)
        {
            using Stream stream = SourceGeneratorUtilsGenerator.Assembly.GetManifestResourceStream(name)!;
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }

    private static string ResourceNameToFileName(string resourceName)
    {
        int startIndex = resourceName switch // do not change the order of the switch arms, we need to check the deepest namespaces first
        {
            _ when resourceName.StartsWith(PolyfillsNamespace) => PolyfillsNamespace.Length,
            _ when resourceName.StartsWith(ExtensionsNamespace) => ExtensionsNamespace.Length,
            _ when resourceName.StartsWith(DescriptorsNamespace) => DescriptorsNamespace.Length,
            _ when resourceName.StartsWith(InfrastructureNamespace) => InfrastructureNamespace.Length,
            _ when resourceName.StartsWith(LocalAssemblyNamespace) => LocalAssemblyNamespace.Length,
            _ => throw new ArgumentException($"The resource name '{resourceName}' does not start with the expected namespace '{LocalAssemblyNamespace}'.", nameof(resourceName))
        };

        ReadOnlySpan<char> fileName = resourceName.AsSpan(startIndex + 1); // +1 to skip the dot
        int lastIndexOfDot = fileName.LastIndexOf('.');

        // extract the file name without the extension
        fileName = fileName[..lastIndexOfDot]; 
        const string genExtension = ".g.cs";

        int fileNameLength = fileName.Length + genExtension.Length;
        StringBuilder sb = new(fileNameLength, fileNameLength);
        sb.AppendSpan(fileName);
        sb.Append(genExtension);

        return sb.ToString();
    }
}