namespace SourceGeneratorUtils;

/// <summary>
/// Provides a base class for source code emitters used within <see cref="TypeSourceFileEmitter{TSpec}"/>.
/// </summary>
public abstract class TypeSourceCodeEmitter<TSpec> : SourceCodeEmitter<TSpec> where TSpec : AbstractTypeGenerationSpec
{
    /// <inheritdoc cref="SourceCodeEmitter{TSpec}.Options"/>
    public new TypeSourceFileEmitterOptions Options => (TypeSourceFileEmitterOptions)base.Options;

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