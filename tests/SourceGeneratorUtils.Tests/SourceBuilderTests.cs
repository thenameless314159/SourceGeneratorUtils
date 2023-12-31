﻿namespace SourceGeneratorUtils.Tests;

public class SourceBuilderTests
{
    [Fact]
    public void Ctor_WithIDictionary_ShouldPopulateSourceFiles()
    {
        var sourceFiles = new Dictionary<string, SourceWriter>
        {
            { "file1.cs", new SourceWriter() },
            { "file2.g.cs", new SourceWriter() }
        };

        var builder = new SourceBuilder(sourceFiles);
        Equal(sourceFiles, builder.SourceFiles);
    }

    [Fact]
    public void Ctor_WithDescriptors_ShouldPopulateSourceFiles()
    {
        var sourceFiles = new SourceFile[]
        {
            new("file1.cs", new SourceWriter()),
            new("file2.g.cs", new SourceWriter())
        };

        var builder = new SourceBuilder(sourceFiles);
        Equal(sourceFiles, builder.SourceFileDescriptors);
    }

    [Theory, InlineData(16), InlineData(128)]
    public void Ctor_WithCapacity_ShouldCreateDictionaryWithGivenCapacity(int capacity)
        => Equal(capacity, new SourceBuilder(capacity).InitialCapacity);

    [Fact]
    public void Register_ShouldAddSingleFileToSourceFiles()
    {
        var (file, fileContent) = ("file.g.cs", new SourceWriter());
        var sourceBuilder = new SourceBuilder();

        sourceBuilder.Register(file, fileContent);

        True(sourceBuilder.SourceFiles.TryGetValue(file, out var content));
        Equal(fileContent, content);
    }

    [Fact]
    public void Register_Descriptor_ShouldAddSingleFileToSourceFiles()
    {
        SourceFile descriptor = new("file.g.cs", new SourceWriter());
        var sourceBuilder = new SourceBuilder();

        sourceBuilder.Register(in descriptor);

        True(sourceBuilder.SourceFiles.TryGetValue(descriptor.Name, out var content));
        Equal(descriptor.Content, content);
    }

    [Fact]
    public void Register_ShouldAddAllFilesToSourceFiles()
    {
        var sourceFiles = new List<KeyValuePair<string, SourceWriter>>
        {
            new("file1.cs", new SourceWriter()), 
            new("file2.cs", new SourceWriter())
        };

        var sourceBuilder = new SourceBuilder();
        sourceBuilder.Register(sourceFiles);

        Equal(sourceFiles.ToDictionary(static x => x.Key, static x => x.Value), sourceBuilder.SourceFiles);
    }

    [Fact]
    public void Register_Array_ShouldAddAllFilesToSourceFiles()
    {
        var sourceFiles = new KeyValuePair<string, SourceWriter>[] 
        {
            new("file1.cs", new SourceWriter()),
            new("file2.cs", new SourceWriter())
        };

        var sourceBuilder = new SourceBuilder();
        sourceBuilder.Register(sourceFiles);

        Equal(sourceFiles.ToDictionary(static x => x.Key, static x => x.Value), sourceBuilder.SourceFiles);
    }

    [Fact]
    public void Register_Descriptors_ShouldAddAllFilesToSourceFiles()
    {
        var sourceFiles = new List<SourceFile>
        {
            new("file1.cs", new SourceWriter()),
            new("file2.cs", new SourceWriter())
        };

        var sourceBuilder = new SourceBuilder();
        sourceBuilder.Register(sourceFiles);

        Equal(sourceFiles.ToDictionary(static x => x.Name, static x => x.Content), sourceBuilder.SourceFiles);
    }

    [Fact]
    public void Register_DescriptorsArray_ShouldAddAllFilesToSourceFiles()
    {
        var sourceFiles = new SourceFile[]
        {
            new("file1.cs", new SourceWriter()),
            new("file2.cs", new SourceWriter())
        };

        var sourceBuilder = new SourceBuilder();
        sourceBuilder.Register(sourceFiles);

        Equal(sourceFiles.ToDictionary(static x => x.Name, static x => x.Content), sourceBuilder.SourceFiles);
    }

    private static readonly Dictionary<string, SourceWriter> _testSourceFiles = new()
    {
        { @"test\\file1.cs", new SourceWriter().WriteLine("HelloWorld!") },
        { "file2.cs", new SourceWriter().WriteLine("Hello There!") }
    };

    [Fact]
    public void ExportTo_ShouldWriteAllFilesToDisk()
    {
        var (file1, file2) = (@"test\\file1.cs", "file2.cs");
        var directory = new DirectoryInfo("tempdir");
        var sourceBuilder = new SourceBuilder(_testSourceFiles);

        sourceBuilder.ExportTo(directory);

        True(File.Exists(Path.Combine(directory.FullName, file1)));
        True(File.Exists(Path.Combine(directory.FullName, file2)));

        var file1Content = File.ReadAllText(Path.Combine(directory.FullName, file1));
        Equal(_testSourceFiles[file1].ToString(), file1Content);

        var file2Content = File.ReadAllText(Path.Combine(directory.FullName, file2));
        Equal(_testSourceFiles[file2].ToString(), file2Content);

        directory.Refresh();
        directory.Delete(true); // cleanup
    }

    [Fact]
    public async Task ExportToAsync_ShouldWriteAllFilesToDisk()
    {
        var (file1, file2) = (@"test\\file1.cs", "file2.cs");
        var directory = new DirectoryInfo("tempdir");
        var sourceBuilder = new SourceBuilder(_testSourceFiles);

        await sourceBuilder.ExportToAsync(directory);

        True(File.Exists(Path.Combine(directory.FullName, file1)));
        True(File.Exists(Path.Combine(directory.FullName, file2)));

        var file1Content =
#if NET6_0_OR_GREATER
            await File.ReadAllTextAsync(Path.Combine(directory.FullName, file1));
#else
            File.ReadAllText(Path.Combine(directory.FullName, file1));
#endif
        Equal(_testSourceFiles[file1].ToString(), file1Content);

        var file2Content =
#if NET6_0_OR_GREATER
            await File.ReadAllTextAsync(Path.Combine(directory.FullName, file2));
#else
            File.ReadAllText(Path.Combine(directory.FullName, file2));
#endif
        Equal(_testSourceFiles[file2].ToString(), file2Content);

        directory.Refresh();
        directory.Delete(true); // cleanup
    }
}