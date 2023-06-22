namespace SourceGeneratorUtils;

/// <summary>
/// Default <see cref="SourceCodeEmitter{TSpec}"/> abstraction to use in <see cref="DefaultSourceFileEmitter"/>.
/// </summary>
public abstract record DefaultSourceCodeEmitter : SourceCodeEmitter<DefaultGenerationSpec>
{
    // Abstraction to reduce verbosity
}