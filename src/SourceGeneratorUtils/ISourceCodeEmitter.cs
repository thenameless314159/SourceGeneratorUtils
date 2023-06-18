namespace SourceGeneratorUtils;

/// <summary>
/// An interface to define logic to emit source code for a given <typeparamref name="TDescriptor"/>.
/// </summary>
/// <typeparam name="TDescriptor">The target specification type to emit source files for.</typeparam>
public interface ISourceCodeEmitter<in TDescriptor>
{
    /// <summary>
    /// Emits the source code for the target <typeparamref name="TDescriptor"/> to the provided <see cref="SourceWriter"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TDescriptor"/> whose source code needs to be emitted.</param>
    /// <param name="writer">The <see cref="SourceWriter"/> used for emitting the target source code.</param>
    void EmitTargetSourceCode(TDescriptor target, SourceWriter writer);
}