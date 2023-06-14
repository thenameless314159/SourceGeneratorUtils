namespace SourceGeneratorUtils;

/// <summary>
/// Provides factory methods for creating <see cref="ISourceFileGenerator{TDescriptor}"/> instances.
/// These methods are meant to be syntax-sugar.
/// </summary>
public static class CSharpSourceGenerator
{
    /// <summary>
    /// Creates a new <see cref="CSharpSourceGenerator{TDescriptor}"/> instance for the given <paramref name="options"/>
    /// using the given <paramref name="configure"/> action to setup the <see cref="CSharpSourceBlockWriter{TDescriptor}"/> used
    /// by this instance.
    /// </summary>
    ///  <typeparam name="TDescriptor">The type of the target descriptor to generate a source file for.</typeparam>
    /// <param name="options">The <see cref="SourceFileGenOptions"/> to use.</param>
    /// <param name="configure">The action to use to setup the <see cref="CSharpSourceBlockWriter{TDescriptor}"/> used by this instance.</param>
    /// <returns>A new <see cref="CSharpSourceGenerator{TDescriptor}"/> instance for the given <paramref name="options"/>.</returns>
    public static ISourceFileGenerator<TDescriptor> Create<TDescriptor>(SourceFileGenOptions options,
        Action<IBuilder<TDescriptor>> configure) where TDescriptor : ITypeSpec
    {
        var builder = new Builder<TDescriptor>();
        configure(builder);

        return new CSharpSourceGenerator<TDescriptor>(options, builder.Build());
    }

    /// <summary>
    /// Represents an interface to fluently register <see cref="CSharpSourceBlockWriter{TDescriptor}"/> instances
    /// to use in the currently built <see cref="CSharpSourceGenerator{TDescriptor}"/>.
    /// </summary>
    /// <typeparam name="TDescriptor">The type of the target descriptor to generate a source file for.</typeparam>
    public interface IBuilder<TDescriptor> where TDescriptor : ITypeSpec
    {
        /// <summary>
        /// Registers a <see cref="CSharpSourceBlockWriter{TDescriptor}"/> to use in the currently built <see cref="CSharpSourceGenerator{TDescriptor}"/>.
        /// </summary>
        /// <param name="blockWriter">The block writer to register.</param>
        /// <returns>The current builder instance.</returns>
        IBuilder<TDescriptor> Use(CSharpSourceBlockWriter<TDescriptor> blockWriter);

        /// <summary>
        /// Registers a <see cref="CSharpSourceBlockWriter{TDescriptor}"/> to use in the currently built <see cref="CSharpSourceGenerator{TDescriptor}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the block writer to register.</typeparam>
        /// <returns>The current builder instance.</returns>
        IBuilder<TDescriptor> Use<T>() where T : CSharpSourceBlockWriter<TDescriptor>, new();
    }

    internal sealed class Builder<TDescriptor> : IBuilder<TDescriptor> 
        where TDescriptor : ITypeSpec
    {
        private readonly List<CSharpSourceBlockWriter<TDescriptor>> _blockWriters = new();

        public IBuilder<TDescriptor> Use(CSharpSourceBlockWriter<TDescriptor> blockWriter)
        {
            _blockWriters.Add(blockWriter);
            return this;
        }

        public IBuilder<TDescriptor> Use<T>() where T : CSharpSourceBlockWriter<TDescriptor>, new()
        {
            _blockWriters.Add(new T());
            return this;
        }

        public IReadOnlyList<CSharpSourceBlockWriter<TDescriptor>> Build() => _blockWriters;
    }
}