using static SourceGeneratorUtils.WellKnownStrings;

namespace SourceGeneratorUtils;

/// <summary>
/// An abstraction encapsulating all the logic necessary to generate ready-to-compile source files for the given target <typeparamref name="TSpec"/>.
/// </summary>
/// <typeparam name="TSpec">The target specification type to emit source files for.</typeparam>
public abstract class SourceFileEmitterBase<TSpec> : ISourceFileGenerator<TSpec>, ISourceCodeEmitter<TSpec>
    where TSpec : AbstractGenerationSpec
{
    /// <summary>
    /// Gets or init a reference to the options to be used for emitting source files within this <see cref="SourceFileEmitterBase{TSpec}"/> instance.
    /// </summary>
    public SourceFileEmitterOptions Options { get; init; }

    /// <summary>
    /// Creates a new <see cref="SourceFileEmitterBase{TSpec}"/> instance with the given <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The <see cref="SourceFileEmitterOptions"/> options.</param>
    protected SourceFileEmitterBase(SourceFileEmitterOptions options) => Options = options;

    /// <summary>
    /// Formats a file name for the given target descriptor.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>The file name of the target <typeparamref name="TSpec"/>.</returns>
    public abstract string GetFileName(TSpec target);

    /// <summary>
    /// Emits the source code for the target <typeparamref name="TSpec"/> to the provided <see cref="SourceWriter"/>.
    /// The <see cref="SourceWriter"/> should be created using the <see cref="CreateSourceWriter"/> method.
    /// This method should be overridden in a derived class to provide the specific logic for emitting the target declaration and body.
    /// The writer will typically be at the type declaration, ready to emit members with custom logic.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/> whose implementation needs to be emitted.</param>
    /// <param name="writer">The <see cref="SourceWriter"/> to use for emitting the target implementation, typically created using <see cref="CreateSourceWriter(TSpec)"/>.</param>
    public abstract void EmitTargetSourceCode(TSpec target, SourceWriter writer);

    /// <summary>
    /// Allows to specify additional outer using directives to apply based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional outer using directives.</returns>
    /// <remarks> Made protected internal for testing purposes.</remarks>
    protected internal virtual IEnumerable<string> GetTargetOuterUsingDirectives(TSpec target) => Enumerable.Empty<string>();

    /// <summary>
    /// Allows to specify additional inner using directives to apply based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional inner using directives.</returns>
    /// <remarks> Made protected internal for testing purposes.</remarks>
    protected internal virtual IEnumerable<string> GetTargetInnerUsingDirectives(TSpec target) => Enumerable.Empty<string>();

    /// <inheritdoc />
    public SourceFile GenerateSource(TSpec target)
    {
        var writer = CreateSourceWriter(target);
        EmitTargetSourceCode(target, writer);

        // Make sure to close any blocks that were left open.
        return new SourceFile(GetFileName(target), writer.CloseAllBlocks());
    }

    /// <summary>
    /// Creates a <see cref="SourceWriter"/> with the appropriate header and using directives for the given target <typeparamref name="TSpec"/>.
    /// This will leave the writer right before the target type declaration allowing for custom logic to be emitted.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/> whose header and optional namespace needs to be emitted.</param>
    /// <returns>A <see cref="SourceWriter"/> at the type declaration.</returns>
    public SourceWriter CreateSourceWriter(TSpec target)
    {
        var writer = new SourceWriter();

        // Emit the source file header first.
        writer.WriteLine(Options.SourceFileHeader ?? ShortSourcePrefix);
        writer.WriteLine();

        // Emit the nullable directive on top.
        writer.WriteLine($"#nullable {EnableOrDisable(Options.EnableNullableAnnotations)} annotations");
        writer.WriteLine($"#nullable {EnableOrDisable(Options.EnableNullableWarnings)} warnings");

        writer.WriteEmptyLines(Options.SpacesBetweenDeclarations);

        // Suppress any warnings specified in options.
        if (Options.SuppressWarnings.Count > 0)
        {
            writer.WriteLine($"#pragma warning disable {string.Join(CommaWithSpace, Options.SuppressWarnings)}");
            writer.WriteEmptyLines(Options.SpacesBetweenDeclarations);
        }

        // Emit the outer using directives if any.
        IReadOnlyList<string> targetOuterUsingDirectives = GetTargetOuterUsingDirectives(target).ToList();
        // review: should we enumerate targetOuterUsingDirectives multiple times with .Any() instead ?
        if (Options.DefaultOuterUsingDirectives.Count > 0 || targetOuterUsingDirectives.Count > 0)
        {
            var outerUsingDirectives = GetDistinctUsingDirectives(
                Options.DefaultOuterUsingDirectives
                    .Concat(targetOuterUsingDirectives));

            writer.WriteLine(string.Join(Environment.NewLine, outerUsingDirectives));
        }

        // Emit the namespace declaration if any.
        if (target.Namespace == null) // review: must check global namespace instead
            return writer;

        writer.WriteEmptyLines(Options.SpacesBetweenDeclarations);

        // review: should add spacing according to options and what was previously emitted
        if (Options.UseFileScopedNamespace) 
            writer
                .WriteLine($"namespace {target.Namespace};")
                .WriteEmptyLines(Options.SpacesBetweenDeclarations);
        else writer
                .WriteLine($"namespace {target.Namespace}")
                .OpenBlock();

        // Emit the inner using directives if a namespace have been declared and if any are present.
        IReadOnlyList<string> targetInnerUsingDirectives = GetTargetInnerUsingDirectives(target).ToList();
        // review: should we enumerate targetInnerUsingDirectives multiple times with .Any() instead ?
        if (Options.DefaultInnerUsingDirectives.Count > 0 || targetInnerUsingDirectives.Count > 0)
        {
            var innerUsingDirectives = GetDistinctUsingDirectives(
                Options.DefaultInnerUsingDirectives
                    .Concat(targetInnerUsingDirectives));

            writer.WriteLine(string.Join(Environment.NewLine, innerUsingDirectives));
            writer.WriteEmptyLines(Options.SpacesBetweenDeclarations);
        }

        return writer;

        static string EnableOrDisable(bool isEnabled) => isEnabled ? enable : disable;
    }

    private static IEnumerable<string> GetDistinctUsingDirectives(IEnumerable<string> namespacesToImport)
        => namespacesToImport
            .Select(StringHelpers.MakeUsingDirective)
            .OrderByDescending(d => d.Length)
            .Distinct();
}