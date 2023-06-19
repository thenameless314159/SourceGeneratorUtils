namespace SourceGeneratorUtils.Tests;

public class KeyValuePairExtensionsTests
{
    [Fact]
    public void WriteToDisk_WithNullDirectory_ShouldWriteToCurrentDire()
    {
        var kvp = new KeyValuePair<string, SourceWriter>(@"test\\file1.cs", 
            new SourceWriter().WriteLine("HelloWorld!"));

        kvp.WriteToDisk(null);
        True(File.Exists(kvp.Key));
        File.Delete(kvp.Key);
    }

    [Fact]
    public void WriteToDisk_ShouldCreateDirectory()
    {
        var kvp = new KeyValuePair<string, SourceWriter>(@"test\\file2.cs", 
            new SourceWriter().WriteLine("Hello World!"));

        var directory = new DirectoryInfo("testDir");

        False(directory.Exists);
        kvp.WriteToDisk(directory);

        directory.Refresh();
        True(directory.Exists);
        directory.Delete(true);
    }
}