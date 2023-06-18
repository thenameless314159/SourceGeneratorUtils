namespace SourceGeneratorUtils;

/// <summary>
/// Represents a source file with its content.
/// </summary>
/// <param name="Name">The file name.</param>
/// <param name="Content">The file content.</param>
public readonly record struct SourceFile(string Name, SourceWriter Content);