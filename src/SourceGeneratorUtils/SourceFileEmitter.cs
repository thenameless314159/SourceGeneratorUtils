namespace SourceGeneratorUtils;

/// <summary>
/// An abstraction relying on <see cref="SourceCodeEmitter{TSpec}"/>s to emit source files for the given target <typeparamref name="TSpec"/>.
/// </summary>
/// <typeparam name="TSpec">The target specification type to emit source files for.</typeparam>
public abstract class SourceFileEmitter<TSpec> : SourceFileEmitterBase<TSpec> where TSpec : AbstractGenerationSpec
{
    internal bool _areOptionsSetup; // internal for testing purposes

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

    /// <inheritdoc/>
    public override void EmitTargetSourceCode(TSpec target, SourceWriter writer)
    {
        using IEnumerator<SourceCodeEmitter<TSpec>> emitters = GetSourceCodeEmitters().GetEnumerator();
        bool hasNext = emitters.MoveNext();

        while (hasNext)
        {
            int lengthBefore = writer.Length; // Store length before emitting to check if anything was emitted
            emitters.Current!.EmitTargetSourceCode(target, writer);

            hasNext = emitters.MoveNext(); // Move to next emitter

            // Emit configured spacing between each emitter if something has been emitted and if we have more to emit
            if (lengthBefore < writer.Length && hasNext) writer.WriteEmptyLines(Options.SpacesBetweenDeclarations);
        }
    }

    /// <inheritdoc/>
    protected internal override IEnumerable<string> GetTargetOuterUsingDirectives(TSpec target) => GetSourceCodeEmitters().SelectMany(e =>
    {
        // Setup the options here since this should be the very first iteration of the source code writers
        // to ensure that the options are properly setup for all emitters.
        if (!_areOptionsSetup)
        {
            e.SetupOptionsIfNone(Options);
            _areOptionsSetup = true;
        }

        return e.GetOuterUsingDirectives(target);
    });

    /// <inheritdoc/>
    protected internal override IEnumerable<string> GetTargetInnerUsingDirectives(TSpec target) 
        => GetSourceCodeEmitters().SelectMany(e => e.GetInnerUsingDirectives(target));
}