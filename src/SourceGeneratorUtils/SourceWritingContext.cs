namespace SourceGeneratorUtils;

/// <summary>
/// Represents the context in which a source file is being generated.
/// Used to generate source blocks via <see cref="ISourceBlockGenerator"/>.
/// </summary>
public readonly record struct SourceWritingContext
{
    /// <summary>
    /// The target <see cref="ITypeSpec"/> to generate source for.
    /// </summary>
    public ITypeSpec Target { get; }

    /// <summary>
    /// The <see cref="SourceFileGenOptions"/> used in this context.
    /// </summary>
    public SourceFileGenOptions Options { get; }

    /// <summary>
    /// The types that are being generated in this context.
    /// </summary>
    public IReadOnlyDictionary<string, ITypeSpec> Types { get; private init; }

    private SourceWritingContext(ITypeSpec target, SourceFileGenOptions options, IReadOnlyDictionary<string, ITypeSpec> types)
        => (Target, Options, Types) = (target, options, types);

    /// <summary>
    /// Creates a new <see cref="SourceWritingContext"/> for the given target type from the given options
    /// and strongly typed <see cref="ITypeSpec"/>.
    /// </summary>
    /// <typeparam name="TDescriptor">The target <see cref="ITypeSpec"/> type.</typeparam>
    /// <param name="target">The target type.</param>
    /// <param name="options">The context options.</param>
    /// <param name="descriptors">The types being generated.</param>
    /// <returns>A new <see cref="SourceWritingContext"/> instance.</returns>
    public static SourceWritingContext CreateFor<TDescriptor>(TDescriptor target, SourceFileGenOptions options, 
        IReadOnlyDictionary<string, TDescriptor>? descriptors = null) where TDescriptor : ITypeSpec 
        => new(target, options, new DescriptorStore<TDescriptor>(descriptors));
}