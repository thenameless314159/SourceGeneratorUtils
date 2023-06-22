using static SourceGeneratorUtils.WellKnownStrings;
using System.Diagnostics;

namespace SourceGeneratorUtils;

/// <summary>
/// An abstraction encapsulating all the logic necessary to generate ready-to-compile source files for the given target <typeparamref name="TSpec"/>.
/// </summary>
/// <remarks>
/// This abstraction relies on <see cref="SourceCodeEmitter{TSpec}"/> components to emit source code for the given target which allows to break down
/// the logic of emitting source code into multiple reusable components. Additionally, this allows a finer control over the source code generation process
/// and simplifies the testing logic of the source code generation logic.
/// </remarks>
/// <typeparam name="TSpec">The target specification type to emit source files for.</typeparam>
public abstract class SourceFileEmitter<TSpec> : SourceFileEmitterBase<TSpec> where TSpec : AbstractTypeGenerationSpec
{
    private readonly IEnumerable<SourceCodeEmitter<TSpec>>? _sourceCodeEmitters;

    /// <inheritdoc cref="SourceFileEmitterBase{TSpec}.Options"/>
    public new SourceFileEmitterOptions Options => (SourceFileEmitterOptions)base.Options;

    /// <summary>
    /// Gets the <see cref="SourceCodeEmitter{TSpec}"/>s to use for emitting source code for the given target <typeparamref name="TSpec"/>.
    /// </summary>
    public IEnumerable<SourceCodeEmitter<TSpec>> SourceCodeEmitters
    {
        get => _sourceCodeEmitters ?? Enumerable.Empty<SourceCodeEmitter<TSpec>>();
        init => _sourceCodeEmitters = Options.InjectOptionsOnCodeEmitters            
            ? value
                .Select(e => !e.HasOptionsSetup() ? e with { Options = Options } : e)
                .ToArray()
            : value;
    }

    /// <summary>
    /// Creates a new <see cref="SourceFileEmitter{TSpec}"/> instance with the given <paramref name="options"/> and <paramref name="codeEmitters"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="SourceCodeEmitter{TSpec}.Options"/> property will be injected from the configured <paramref name="options"/>
    /// for all the configured <paramref name="codeEmitters"/> if they don't have any options configured.
    /// </remarks>
    /// <param name="options">The <see cref="SourceFileEmitterOptions"/> options.</param>
    /// <param name="codeEmitters">The <see cref="SourceCodeEmitter{TSpec}"/> components.</param>
    protected SourceFileEmitter(SourceFileEmitterOptions options, IReadOnlyList<SourceCodeEmitter<TSpec>> codeEmitters) : base(options)
    {
        if (!options.InjectOptionsOnCodeEmitters)
        {
            _sourceCodeEmitters = codeEmitters;
            return;
        }

        SourceCodeEmitter<TSpec>[] copy = new SourceCodeEmitter<TSpec>[codeEmitters.Count];
        for (int i = 0; i < codeEmitters.Count; i++)
        {
            SourceCodeEmitter<TSpec> codeEmitter = codeEmitters[i];

            copy[i] = !codeEmitter.HasOptionsSetup() 
                ? codeEmitter with { Options = options }
                : codeEmitter;
        }

        _sourceCodeEmitters = copy;
    }

    /// <summary>
    /// Creates a new <see cref="SourceFileEmitter{TSpec}"/> class with the default <see cref="SourceFileEmitterOptions"/> and the specified <paramref name="codeEmitters"/>.
    /// </summary>
    /// <param name="codeEmitters">The <see cref="SourceCodeEmitter{TSpec}"/>s components.</param>
    protected SourceFileEmitter(IReadOnlyList<SourceCodeEmitter<TSpec>> codeEmitters) : this(SourceFileEmitterOptions.Default, codeEmitters)
    {
    }

    /// <summary>
    /// Creates a new <see cref="SourceFileEmitter{TSpec}"/> instance with the given <paramref name="options"/> and no <see cref="SourceCodeEmitter{TSpec}"/>.
    /// </summary>
    /// <param name="options">The <see cref="SourceFileEmitterOptions"/> options.</param>
    protected SourceFileEmitter(SourceFileEmitterOptions options) : base(options)
    {
    }

    /// <summary>
    /// Creates a new <see cref="SourceFileEmitter{TSpec}"/> instance with the default <see cref="SourceFileEmitterOptions"/> and no <see cref="SourceCodeEmitter{TSpec}"/>.
    /// </summary>
    protected SourceFileEmitter() : base(SourceFileEmitterOptions.Default)
    {
    }

    /// <summary>
    /// Emits the source code for the target <typeparamref name="TSpec"/> to the provided <see cref="SourceWriter"/> based on the configured <see cref="SourceCodeEmitter{TSpec}"/>s.
    /// The <see cref="SourceWriter"/> should be created using the <see cref="SourceFileEmitterBase{TSpec}.CreateSourceWriter"/> method.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/> whose implementation needs to be emitted.</param>
    /// <param name="writer">The <see cref="SourceWriter"/> to use for emitting the target implementation, typically created using <see cref="SourceFileEmitterBase{TSpec}.CreateSourceWriter(TSpec)"/>.</param>
    public override void EmitTargetSourceCode(TSpec target, SourceWriter writer)
    {
        using IEnumerator<SourceCodeEmitter<TSpec>> emitters = SourceCodeEmitters.GetEnumerator();
        bool hasNext = emitters.MoveNext();

        while (hasNext)
        {
            bool shouldAppendEmptyLines = false;
            
            SourceCodeEmitter<TSpec> emitter = emitters.Current!;
            if (emitter.CanEmitTargetSourceCode(target))
            {
                int lengthBefore = writer.Length; // store the length before emitting to check if anything was emitted
                emitter.EmitTargetSourceCode(target, writer);
                shouldAppendEmptyLines = writer.Length > lengthBefore; // only if something has been emitted
            }

            hasNext = emitters.MoveNext(); // Move to next emitter

            // Emit configured blank lines between each emitter
            if (shouldAppendEmptyLines && hasNext) // only if something has been emitted and if we have more to emit
                writer.WriteEmptyLines(Options.BlankLinesBetweenCodeEmitters);
        }
    }

    /// <summary>
    /// Creates a <see cref="SourceWriter"/> with the appropriate header and type declarations for the given target <typeparamref name="TSpec"/>.
    /// This will leave the writer right after the target type declaration allowing to emit the body of the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/> whose header, optional namespace and type declarations needs to be emitted.</param>
    /// <returns>A <see cref="SourceWriter"/> at the type body declaration.</returns>
    public override SourceWriter CreateSourceWriter(TSpec target)
    {
        var writer = base.CreateSourceWriter(target);

        // Emit the classes declarations.
        ImmutableEquatableArray<string> specClasses = target.TypeDeclarations;
        Debug.Assert(specClasses.Count > 0);

        // Emit any containing classes first.
        for (int i = specClasses.Count - 1; i > 0; i--)
        {
            writer.WriteLine(specClasses[i]);
            writer.OpenBlock();
        }
        
        // Gather the interfaces and base type to implement on the target class declaration.
        IReadOnlyList<string> targetInterfacesToImplement = GetTargetInterfacesToImplement(target).ToList();
        string? interfacesToImplement = Options.DefaultInterfaces.Count > 0 || targetInterfacesToImplement.Count > 0
            ? string.Join(", ", Options.DefaultInterfaces.Concat(targetInterfacesToImplement))
            : null;

        bool hasInterfacesToImplement = !string.IsNullOrWhiteSpace(interfacesToImplement);

        string targetDeclaration = specClasses[0];
        Debug.Assert(!string.IsNullOrWhiteSpace(targetDeclaration));

        int lastIndexOfNewLine = targetDeclaration.LastIndexOf(Environment.NewLine, StringComparison.InvariantCulture);
        string? targetTypeDeclarationHeader = lastIndexOfNewLine != -1
            ? targetDeclaration[..lastIndexOfNewLine]
            : null;

        // Emit the target type declaration header.
        if (targetTypeDeclarationHeader != null)
            writer.WriteLine(targetTypeDeclarationHeader);

        IReadOnlyList<string> targetAttributesToApply = Options.AssemblyName != null
            ? GetTargetAttributesToApply(target).Append($"global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{Options.AssemblyName.Name}\", \"{Options.AssemblyName.Version}\")").ToList()
            : GetTargetAttributesToApply(target).ToList();

        // Apply the attributes on the target class if any.
        if (Options.DefaultAttributes.Count > 0 || targetAttributesToApply.Count > 0)
        {
            IEnumerable<string> attributes = Options.DefaultAttributes.Concat(targetAttributesToApply);
            string attributesToApply = !Options.UseCombinedAttributeDeclaration
                ? string.Join(Environment.NewLine, attributes.Select(static a => $"[{a}]"))
                : $"[{string.Join(", ", attributes)}]";

            writer.WriteLine(attributesToApply);
        }
        
        string targetTypeDeclaration = lastIndexOfNewLine != -1
            ? targetDeclaration[(lastIndexOfNewLine + Environment.NewLine.Length)..]
            : targetDeclaration;

        // The target doesn't have any declared base type
        if (targetTypeDeclaration.IndexOf(':') == -1)
        {
            bool hasBaseTypeToInheritFrom = !string.IsNullOrWhiteSpace(Options.DefaultBaseType);
            bool hasBoth = hasBaseTypeToInheritFrom && hasInterfacesToImplement;

            string baseTypeWithInterfaces = hasBaseTypeToInheritFrom || hasInterfacesToImplement
                ? $" : {Options.DefaultBaseType ?? string.Empty}{SeparatorOrEmpty(hasBoth, CommaWithSpace)}{interfacesToImplement}"
                : string.Empty;

            writer.WriteLine($"{targetTypeDeclaration}{baseTypeWithInterfaces}");
            writer.OpenBlock();
            return writer;
        }

        // Emit the target class declaration with interfaces only.
        writer.WriteLine($"{targetTypeDeclaration}{interfacesToImplement}");
        writer.OpenBlock();

        return writer;

        static string SeparatorOrEmpty(bool returnSeparator, string separator) => returnSeparator ? separator : string.Empty;
    }

    /// <summary>
    /// Allows to specify additional outer using directives to apply based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional outer using directives.</returns>
    public override IEnumerable<string> GetTargetOuterUsingDirectives(TSpec target) 
        => SourceCodeEmitters.SelectMany(e => e.GetOuterUsingDirectives(target));

    /// <summary>
    /// Allows to specify additional inner using directives to apply based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional inner using directives.</returns>
    public override IEnumerable<string> GetTargetInnerUsingDirectives(TSpec target) 
        => SourceCodeEmitters.SelectMany(e => e.GetInnerUsingDirectives(target));

    /// <summary>
    /// Allows to specify additional attributes to apply based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional attributes.</returns>
    public virtual IEnumerable<string> GetTargetAttributesToApply(TSpec target)
        => SourceCodeEmitters.SelectMany(e => e.GetAttributesToApply(target));

    /// <summary>
    /// Allows to specify additional interfaces to implements based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional additional interfaces to implement.</returns>
    public virtual IEnumerable<string> GetTargetInterfacesToImplement(TSpec target)
        => SourceCodeEmitters.SelectMany(e => e.GetInterfacesToImplement(target));
}