namespace SourceGeneratorUtils;

/// <summary>
/// Default abstraction for <see cref="ISourceCodeEmitter{TDescriptor}"/> using an implementation of the <see cref="AbstractTypeGenerationSpec"/>
/// to allow breaking down source code generation logic into multiple reusable components.
/// </summary>
/// <remarks>
/// This type has been made a record to allow cloning the instance with a different <see cref="Options"/> instance.
/// </remarks>
/// <typeparam name="TSpec">The target specification type to emit source files for.</typeparam>
public abstract record SourceCodeEmitter<TSpec> : ISourceCodeEmitter<TSpec> where TSpec : AbstractTypeGenerationSpec
{
    internal bool HasOptionsSetup() => _options != null;
    private readonly SourceFileEmitterOptions? _options;

    /// <summary>
    /// Gets or init a reference to the options to be used for emitting source files within this <see cref="SourceCodeEmitter{TSpec}"/> instance.
    /// </summary>
    /// <remarks>
    /// This property is typically injected by the <see cref="SourceFileEmitter{TSpec}"/> that uses this <see cref="SourceCodeEmitter{TSpec}"/> implementation
    /// if its <see cref="SourceFileEmitterOptions.InjectOptionsOnCodeEmitters"/> was set to true.
    /// </remarks>
    public SourceFileEmitterOptions Options
    {
        get => _options ?? throw new InvalidOperationException("Missing options ! Make sure to setup properly either via ctor or init property accessor."); 
        init => _options = value;
    }

    /// <summary>
    /// Creates a new <see cref="SourceCodeEmitter{TSpec}"/> instance with the given <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The <see cref="SourceFileEmitterOptions"/> to use.</param>
    protected SourceCodeEmitter(SourceFileEmitterOptions options) => _options = options;

    /// <summary>
    /// Creates a new <see cref="SourceCodeEmitter{TSpec}"/> instance with no <see cref="SourceFileEmitterOptions"/>.
    /// (use this constructor if you want the options injected from <see cref="SourceFileEmitter{TSpec}"/>)
    /// </summary>
    protected SourceCodeEmitter() => _options = null;

    /// <summary>
    /// Emits the source code for the target <typeparamref name="TSpec"/> to the specified <see cref="SourceWriter"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/> whose source code needs to be emitted.</param>
    /// <param name="writer">The <see cref="SourceWriter"/> to use for emitting the target source code.</param>
    public abstract void EmitTargetSourceCode(TSpec target, SourceWriter writer);

    /// <summary>
    /// Allows to specify whether the source code for the target <typeparamref name="TSpec"/> should be emitted or not.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/> whose source code may not be emitted.</param>
    /// <returns>Whether the source code for the target <typeparamref name="TSpec"/> should be emitted or not.</returns>
    public virtual bool CanEmitTargetSourceCode(TSpec target) => true;

    /// <summary>
    /// Allows to specify additional outer using directives to apply based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional outer using directives.</returns>
    public virtual IEnumerable<string> GetOuterUsingDirectives(TSpec target) => Enumerable.Empty<string>();

    /// <summary>
    /// Allows to specify additional inner using directives to apply based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional inner using directives.</returns>
    public virtual IEnumerable<string> GetInnerUsingDirectives(TSpec target) => Enumerable.Empty<string>();

    /// <summary>
    /// Allows to specify additional attributes to apply on the target type declaration based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional attributes.</returns>
    public virtual IEnumerable<string> GetAttributesToApply(TSpec target) => Enumerable.Empty<string>();

    /// <summary>
    /// Allows to specify additional interfaces to implement on the target type declaration based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional interfaces.</returns>
    public virtual IEnumerable<string> GetInterfacesToImplement(TSpec target) => Enumerable.Empty<string>();
}