namespace SourceGeneratorUtils;

/// <summary>
/// Represents a source file descriptor.
/// </summary>
/// <param name="Name">The file name.</param>
/// <param name="Content">The file content.</param>
public readonly record struct SourceFileDescriptor(string Name, SourceWriter Content);