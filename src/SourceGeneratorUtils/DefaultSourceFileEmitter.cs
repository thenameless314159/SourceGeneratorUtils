namespace SourceGeneratorUtils;

/// <summary>
/// Provides a default implementation of <see cref="SourceFileEmitter{TDescriptor}"/> that uses <see cref="DefaultGenerationSpec"/> as the descriptor.
/// </summary>
public class DefaultSourceFileEmitter : SourceFileEmitter<DefaultGenerationSpec>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultSourceFileEmitter"/> class with the specified <paramref name="options"/> and <paramref name="codeEmitters"/>.
    /// </summary>
    /// <param name="options">The options to use to generate source files.</param>
    /// <param name="codeEmitters">The <see cref="DefaultSourceCodeEmitter"/>s to use to generate source code.</param>
    public DefaultSourceFileEmitter(SourceFileEmitterOptions options, params DefaultSourceCodeEmitter[] codeEmitters) : base(options, codeEmitters)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultSourceFileEmitter"/> class with the default <see cref="SourceFileEmitterOptions"/> and the specified <paramref name="codeEmitters"/>.
    /// </summary>
    /// <param name="codeEmitters">The <see cref="DefaultSourceCodeEmitter"/>s to use to generate source code.</param>
    public DefaultSourceFileEmitter(IReadOnlyList<DefaultSourceCodeEmitter> codeEmitters) : base(codeEmitters)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultSourceFileEmitter"/> class with the default <see cref="SourceFileEmitterOptions"/>.
    /// </summary>
    public DefaultSourceFileEmitter() : base(SourceFileEmitterOptions.Default)
    {
    }

    /// <inheritdoc/>
    public override string GetFileName(DefaultGenerationSpec target) => $"{target.TargetType.Name}.g.cs";
}