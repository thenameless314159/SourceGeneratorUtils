using static SourceGeneratorUtils.WellKnownStrings;
using System.Diagnostics;

namespace SourceGeneratorUtils;

/// <summary>
/// Provides another <see cref="SourceFileEmitter{TSpec}"/> abstraction using an implementation of <see cref="AbstractTypeGenerationSpec"/>.
/// This abstraction takes care of the types declarations and allows to specify additional attributes and interfaces to apply on the target type declaration.
/// </summary>
public abstract class TypeSourceFileEmitter<TSpec> : SourceFileEmitter<TSpec> where TSpec : AbstractTypeGenerationSpec
{
    /// <inheritdoc cref="SourceFileEmitterBase{TSpec}.Options"/>
    public new TypeSourceFileEmitterOptions Options => (TypeSourceFileEmitterOptions)base.Options;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeSourceFileEmitter{TSpec}"/> class with the given <see cref="TypeSourceFileEmitterOptions"/>.
    /// </summary>
    /// <param name="options">The options to use to generate source files.</param>
    protected TypeSourceFileEmitter(TypeSourceFileEmitterOptions options) : base(options)
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
    public abstract IEnumerable<TypeSourceCodeEmitter<TSpec>> GetTypeSourceCodeEmitters();


    /// <summary>
    /// Allows to specify additional attributes to apply based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional attributes.</returns>
    protected internal virtual IEnumerable<string> GetTargetAttributesToApply(TSpec target)
        => GetTypeSourceCodeEmitters().SelectMany(e => e.GetAttributesToApply(target));

    /// <summary>
    /// Allows to specify additional interfaces to implements based on the target <typeparamref name="TSpec"/>.
    /// </summary>
    /// <param name="target">The target <typeparamref name="TSpec"/>.</param>
    /// <returns>A list of the additional additional interfaces to implement.</returns>
    protected internal virtual IEnumerable<string> GetTargetInterfacesToImplement(TSpec target)
        => GetTypeSourceCodeEmitters().SelectMany(e => e.GetInterfacesToImplement(target));

    /// <inheritdoc />
    public override IEnumerable<SourceCodeEmitter<TSpec>> GetSourceCodeEmitters() => GetTypeSourceCodeEmitters();

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

        // Emit the attributes to apply on the target class if any.
        IReadOnlyList<string> targetAttributesToApply = GetTargetAttributesToApply(target).ToList();
        if (Options.DefaultAttributes.Count > 0 || targetAttributesToApply.Count > 0)
        {
            var attributes = Options.DefaultAttributes.Concat(targetAttributesToApply);
            writer.WriteLine(
                Options.UseCombinedAttributeDeclaration
                    ? $"[{string.Join(", ", attributes)}]"
                    : string.Join(Environment.NewLine, attributes.Select(static a => $"[{a}]")));
        }

        // Emit the GeneratedCodeAttribute if needed.
        if (Options.AssemblyName != null)
        {
            // Annotate the target class with the GeneratedCodeAttribute
            writer.WriteLine($"""[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{Options.AssemblyName.Name}", "{Options.AssemblyName.Version}")]""");
        }

        // Gather the interfaces and base type to implement on the target class declaration.
        IReadOnlyList<string> targetInterfacesToImplement = GetTargetInterfacesToImplement(target).ToList();
        string? interfacesToImplement = Options.DefaultInterfaces.Count > 0 || targetInterfacesToImplement.Count > 0
            ? string.Join(", ", Options.DefaultInterfaces.Concat(targetInterfacesToImplement))
            : null;

        bool hasInterfacesToImplement = !string.IsNullOrWhiteSpace(interfacesToImplement);
        string targetDeclaration = specClasses[0];


        // The target doesn't have any base declaration
        if (targetDeclaration.IndexOf(':') == -1) 
        {
            bool hasBaseTypeToInheritFrom = !string.IsNullOrWhiteSpace(Options.DefaultBaseType);
            bool hasBoth = hasBaseTypeToInheritFrom && hasInterfacesToImplement;

            string baseTypeWithInterfaces = hasBaseTypeToInheritFrom || hasInterfacesToImplement
                ? $" : {Options.DefaultBaseType ?? string.Empty}{SeparatorOrEmpty(hasBoth, CommaWithSpace)}{interfacesToImplement}"
                : string.Empty;

            // Emit the target class declaration with base type and interfaces.
            writer.WriteLine($"{targetDeclaration}{baseTypeWithInterfaces}");
            writer.OpenBlock();
            return writer;
        }

        // Emit the target class declaration with interfaces only.
        // review: may check later whether the already declared base is an interface or not but how ?
        //         Some non-interfaces types may start with the letter I like Index or ImmutableArray...
        writer.WriteLine($"{targetDeclaration}{SeparatorOrEmpty(hasInterfacesToImplement, CommaWithSpace)}{interfacesToImplement}");
        writer.OpenBlock();
        
        return writer;

        static string SeparatorOrEmpty(bool returnSeparator, string separator) => returnSeparator ? separator : string.Empty;
    }
}