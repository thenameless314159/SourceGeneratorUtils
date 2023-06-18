namespace SourceGeneratorUtils;

/// <summary>
/// Provides a base class for type source code emitters used within <see cref="TypeSourceFileEmitter"/>.
/// </summary>
public abstract class TypeSourceCodeEmitter : SourceCodeEmitter<TypeGenerationSpec>
{
    /// <inheritdoc cref="SourceCodeEmitter{TSpec}.Options"/>
    public new TypeSourceFileEmitterOptions Options => (TypeSourceFileEmitterOptions)base.Options;

    /// <summary>
    /// Allows to specify additional attributes to apply on the target type declaration based on the target <see cref="TypeGenerationSpec"/>.
    /// </summary>
    /// <param name="target">The target <see cref="TypeGenerationSpec"/>.</param>
    /// <returns>A list of the additional attributes.</returns>
    public virtual IEnumerable<string> GetAttributesToApply(TypeGenerationSpec target) => Enumerable.Empty<string>();

    /// <summary>
    /// Allows to specify additional interfaces to implement on the target type declaration based on the target <see cref="TypeGenerationSpec"/>.
    /// </summary>
    /// <param name="target">The target <see cref="TypeGenerationSpec"/>.</param>
    /// <returns>A list of the additional interfaces.</returns>
    public virtual IEnumerable<string> GetInterfacesToImplement(TypeGenerationSpec target) => Enumerable.Empty<string>();
}