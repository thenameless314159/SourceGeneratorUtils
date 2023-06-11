namespace SourceGeneratorUtils;

/// <summary>
/// A thin wrapper over a <see cref="Dictionary{TKey, TValue}"/> of file names and <see cref="SourceWriter"/>
/// to store generated source files and export them to disk.
/// </summary>
public class SourceBuilder
{
    protected internal readonly Dictionary<string, SourceWriter> _sourceFiles;

    /// <summary>
    /// Gets the generated source files.
    /// </summary>
    public IReadOnlyDictionary<string, SourceWriter> SourceFiles => _sourceFiles;

    /// <summary>
    /// Gets the initial capacity of the underlying store.
    /// Returns 0 if no initial capacity was specified.
    /// </summary>
    public int InitialCapacity { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceBuilder"/> class with the given initial source files.
    /// </summary>
    /// <param name="sourceFiles">The initial source files.</param>
    public SourceBuilder(IDictionary<string, SourceWriter> sourceFiles) => _sourceFiles = new(sourceFiles);

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceBuilder"/> class with the given initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity for the underlying store.</param>
    public SourceBuilder(int capacity) => (InitialCapacity, _sourceFiles) = (capacity, new(capacity));

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceBuilder"/> class with default capacity.
    /// </summary>
    public SourceBuilder() => _sourceFiles = new();

    /// <summary>
    /// Registers the specified source files into the current <see cref="SourceBuilder"/>.
    /// </summary>
    /// <param name="sourceFiles">The source files to register.</param>
    /// <returns>A self <see cref="SourceBuilder"/> instance to chain calls.</returns>
    public SourceBuilder Register(IEnumerable<KeyValuePair<string, SourceWriter>> sourceFiles)
    {
        // fast path if the sourceFiles are contained within an array
        if (sourceFiles is KeyValuePair<string, SourceWriter>[] sourceFileArray) 
        {
            foreach (ref readonly KeyValuePair<string, SourceWriter> kvp in sourceFileArray.AsSpan())
                _sourceFiles[kvp.Key] = kvp.Value;
        }
        else
        {
            foreach (KeyValuePair<string, SourceWriter> kvp in sourceFiles)
                _sourceFiles[kvp.Key] = kvp.Value;
        }

        return this;
    }

    /// <summary>
    /// Registers the given source file into the current <see cref="SourceBuilder"/>.
    /// </summary>
    /// <param name="fileName">The name of the source file.</param>
    /// <param name="fileContent">The source file content.</param>
    /// <returns>A self <see cref="SourceBuilder"/> instance to chain calls.</returns>
    public SourceBuilder Register(string fileName, SourceWriter fileContent)
    {
        _sourceFiles[fileName] = fileContent;
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
        foreach (KeyValuePair<string, SourceWriter> kvp in _sourceFiles)
        {
            WriteToDisk(in kvp, directory);
        }
    }

    private static void WriteToDisk(in KeyValuePair<string, SourceWriter> kvp, DirectoryInfo? directory = null)
    {
        var file = directory != null 
            ? new FileInfo(Path.Combine(directory.FullName, kvp.Key)) 
            : new FileInfo(kvp.Key);

        file.Directory?.Create();
        File.WriteAllText(file.FullName, kvp.Value.ToString());
    }
}