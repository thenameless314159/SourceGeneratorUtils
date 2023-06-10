namespace SourceGeneratorUtils;

/// <summary>
/// A thin wrapper over <see cref="Dictionary{TKey, TValue}"/> that uses <see cref="FileInfoEqualityComparer"/> as comparer
/// to store generated source files and export them to disk.
/// </summary>
public class SourceBuilder
{
    protected internal readonly Dictionary<FileInfo, SourceWriter> _sourceFiles;

    /// <summary>
    /// Gets the generated source files.
    /// </summary>
    public IReadOnlyDictionary<FileInfo, SourceWriter> SourceFiles => _sourceFiles;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceBuilder"/> class with the given initial source files.
    /// </summary>
    /// <param name="sourceFiles">The initial source files.</param>
    public SourceBuilder(IDictionary<FileInfo, SourceWriter> sourceFiles) => _sourceFiles = new(sourceFiles, FileInfoEqualityComparer.Instance);

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceBuilder"/> class with the given initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity for the underlying store.</param>
    public SourceBuilder(int capacity) => _sourceFiles = new(capacity, FileInfoEqualityComparer.Instance);

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceBuilder"/> class with default capacity.
    /// </summary>
    public SourceBuilder() => _sourceFiles = new(FileInfoEqualityComparer.Instance);

    /// <summary>
    /// Registers the specified source files into the current <see cref="SourceBuilder"/>.
    /// </summary>
    /// <param name="sourceFiles">The source files to register.</param>
    /// <returns>A self <see cref="SourceBuilder"/> instance to chain calls.</returns>
    public SourceBuilder Register(IEnumerable<KeyValuePair<FileInfo, SourceWriter>> sourceFiles)
    {
        // fast path if the sourceFiles are contained within an array
        if (sourceFiles is KeyValuePair<FileInfo, SourceWriter>[] sourceFileArray) 
        {
            foreach (ref readonly KeyValuePair<FileInfo, SourceWriter> kvp in sourceFileArray.AsSpan())
                _sourceFiles[kvp.Key] = kvp.Value;
        }
        else
        {
            foreach (KeyValuePair<FileInfo, SourceWriter> kvp in sourceFiles)
                _sourceFiles[kvp.Key] = kvp.Value;
        }

        return this;
    }

    /// <summary>
    /// Registers the given source file into the current <see cref="SourceBuilder"/>.
    /// </summary>
    /// <param name="file">The <see cref="FileInfo"/>.</param>
    /// <param name="sourceWriter">The source file content.</param>
    /// <returns>A self <see cref="SourceBuilder"/> instance to chain calls.</returns>
    public SourceBuilder Register(FileInfo file, SourceWriter sourceWriter)
    {
        _sourceFiles[file] = sourceWriter;
        return this;
    }

    /// <summary>
    /// Exports the generated source files to the specified directory using <see cref="Parallel"/>.ForEach method.
    /// </summary>
    /// <param name="directory">The directory where the files should be exported.</param>
    /// <returns>The <see cref="ValueTask"/>.</returns>
    public ValueTask ExportToAsync(DirectoryInfo directory)
    {
        _ = Parallel.ForEach(_sourceFiles, kvp => WriteToDisk(in kvp, directory));
        return default;
    }

    /// <summary>
    /// Exports the generated source files to the specified directory.
    /// </summary>
    /// <param name="directory">The directory where the files should be exported.</param>
    public void ExportTo(DirectoryInfo directory)
    {
        foreach (KeyValuePair<FileInfo, SourceWriter> kvp in _sourceFiles)
        {
            WriteToDisk(in kvp, directory);
        }
    }

    private static void WriteToDisk(in KeyValuePair<FileInfo, SourceWriter> kvp, DirectoryInfo? directory = null)
    {
        var file = directory != null ? new FileInfo(Path.Combine(directory.FullName, kvp.Key.FullName)) : kvp.Key;

        file.Directory?.Create();
        File.WriteAllText(file.FullName, kvp.Value.ToString());
    }
}