namespace SourceGeneratorUtils;

internal static class KeyValuePairExtensions
{
    public static void WriteToDisk(this in KeyValuePair<string, SourceWriter> kvp, DirectoryInfo? directory = null)
    {
        var file = directory != null
            ? new FileInfo(Path.Combine(directory.FullName, kvp.Key))
            : new FileInfo(kvp.Key);

        file.Directory?.Create();
        File.WriteAllText(file.FullName, kvp.Value.ToString());
    }
}