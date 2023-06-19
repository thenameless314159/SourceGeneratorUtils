namespace SourceGeneratorUtils;

/// <summary>
/// An abstraction relying on <see cref="SourceCodeEmitter{TSpec}"/>s to emit source files for the given target <typeparamref name="TSpec"/>
/// using configured <see cref="SourceCodeEmitter{TSpec}"/>s to emit source code.
/// </summary>
/// <typeparam name="TSpec">The target specification type to emit source files for.</typeparam>
public abstract class SourceFileEmitter<TSpec> : SourceFileEmitterBase<TSpec> where TSpec : AbstractGenerationSpec
{
    /// <summary>
    /// The number of blank lines to add between each source code writers.
    /// </summary>
    public int BlankLinesBetweenSourceCodeWriters { get; init; } = 1;

    /// <inheritdoc/>
    protected SourceFileEmitter(SourceFileEmitterOptions options) : base(options)
    {
    }

    /// <summary>
    /// Gets the <see cref="SourceCodeEmitter{TSpec}"/>s to use for emitting source code for the given target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <returns>An enumerable of <see cref="SourceCodeEmitter{TSpec}"/> instances.</returns>
    /// <remarks>
    /// This will be called several times during the generation process,
    /// please make sure to implement from an immutable or static enumerable.
    /// </remarks>
    public abstract IEnumerable<SourceCodeEmitter<TSpec>> GetSourceCodeEmitters();

    /// <summary>
    /// Emits the source code for the target <typeparamref name="TSpec"/> to the provided <see cref="SourceWriter"/> based on the configured <see cref="GetSourceCodeEmitters"/> result.
    /// The <see cref="SourceWriter"/> should be created using the <see cref="SourceFileEmitterBase{TSpec}.CreateSourceWriter"/> method.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/> whose implementation needs to be emitted.</param>
    /// <param name="writer">The <see cref="SourceWriter"/> to use for emitting the target implementation, typically created using <see cref="SourceFileEmitterBase{TSpec}.CreateSourceWriter(TSpec)"/>.</param>
    public override void EmitTargetSourceCode(TSpec target, SourceWriter writer)
    {
        using IEnumerator<SourceCodeEmitter<TSpec>> emitters = GetSourceCodeEmitters().GetEnumerator();
        bool hasNext = emitters.MoveNext();

        while (hasNext)
        {
            int lengthBefore = writer.Length; // Store length before emitting to check if anything was emitted
            emitters.Current!.EmitTargetSourceCode(target, writer);

            hasNext = emitters.MoveNext(); // Move to next emitter

            // Emit configured blank lines between each emitter if something has been emitted and if we have more to emit
            if (writer.Length > lengthBefore && hasNext) writer.WriteEmptyLines(BlankLinesBetweenSourceCodeWriters);
        }
    }

    /// <summary>
    /// Allows to specify additional outer using directives to apply based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional outer using directives.</returns>
    /// <remarks> Made protected internal for testing purposes.</remarks>
    protected internal override IEnumerable<string> GetTargetOuterUsingDirectives(TSpec target) => GetSourceCodeEmitters().SelectMany(e =>
    {
        // Setup the options here since this should be the very first iteration of the source code writers
        // to ensure that the options are properly setup for all emitters.
        e.SetupOptionsIfNone(Options); // review: kind of a dirty trick, should find a cleaner way to ensure this

        return e.GetOuterUsingDirectives(target);
    });

    /// <summary>
    /// Allows to specify additional inner using directives to apply based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional inner using directives.</returns>
    /// <remarks> Made protected internal for testing purposes.</remarks>
    protected internal override IEnumerable<string> GetTargetInnerUsingDirectives(TSpec target) 
        => GetSourceCodeEmitters().SelectMany(e => e.GetInnerUsingDirectives(target));
}