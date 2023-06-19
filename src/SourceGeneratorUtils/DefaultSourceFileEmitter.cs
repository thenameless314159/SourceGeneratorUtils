namespace SourceGeneratorUtils;

/// <summary>
/// Provides a default implementation of <see cref="TypeSourceFileEmitter{TDescriptor}"/> that uses <see cref="DefaultGenerationSpec"/> as the descriptor.
/// </summary>
public class DefaultSourceFileEmitter : TypeSourceFileEmitter<DefaultGenerationSpec>
{
    /// <summary>
    /// Gets or sets the <see cref="DefaultSourceCodeEmitter"/>s to use to generate source code.
    /// </summary>
    public IReadOnlyList<DefaultSourceCodeEmitter> SourceCodeEmitters { get; init; } = Array.Empty<DefaultSourceCodeEmitter>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultSourceFileEmitter"/> class with the specified <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The options to use to generate source files.</param>
    public DefaultSourceFileEmitter(TypeSourceFileEmitterOptions options) : base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultSourceFileEmitter"/> class with the default <see cref="TypeSourceFileEmitterOptions"/>.
    /// </summary>
    public DefaultSourceFileEmitter() : this(TypeSourceFileEmitterOptions.Default)
    {
    }

    /// <inheritdoc/>
    public override string GetFileName(DefaultGenerationSpec target) => $"{target.TargetType.Name}.g.cs";

    /// <inheritdoc/>
    public override IEnumerable<TypeSourceCodeEmitter<DefaultGenerationSpec>> GetTypeSourceCodeEmitters()
        => SourceCodeEmitters;
}