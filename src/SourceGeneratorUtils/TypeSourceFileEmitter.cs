using static SourceGeneratorUtils.WellKnownStrings;
using System.Diagnostics;

namespace SourceGeneratorUtils;

/// <summary>
/// Provides a default implementation of <see cref="SourceFileEmitter{TSpec}"/> using <see cref="TypeGenerationSpec"/>.
/// </summary>
public class TypeSourceFileEmitter : SourceFileEmitter<TypeGenerationSpec>
{
    /// <inheritdoc cref="SourceFileEmitterBase{TSpec}.Options"/>
    public new TypeSourceFileEmitterOptions Options => (TypeSourceFileEmitterOptions)base.Options;

    /// <summary>
    /// Gets or init the list of <see cref="TypeSourceCodeEmitter"/> to use to generate source code.
    /// </summary>
    public IReadOnlyList<TypeSourceCodeEmitter> Emitters { get; init; } = Array.Empty<TypeSourceCodeEmitter>();

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeSourceFileEmitter"/> class with the given <see cref="TypeSourceFileEmitterOptions"/>.
    /// </summary>
    /// <param name="options">The options to use to generate source files.</param>
    public TypeSourceFileEmitter(TypeSourceFileEmitterOptions options) : base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeSourceFileEmitter"/> class with the default <see cref="TypeSourceFileEmitterOptions"/>.
    /// </summary>
    public TypeSourceFileEmitter() : this(TypeSourceFileEmitterOptions.Default)
    {
    }

    /// <summary>
    /// Allows to specify additional attributes to apply based on the target <see cref="TypeGenerationSpec"/>.
    /// </summary>
    /// <param name="target">The target <see cref="TypeGenerationSpec"/>.</param>
    /// <returns>A list of the additional attributes.</returns>
    protected internal virtual IEnumerable<string> GetTargetAttributesToApply(TypeGenerationSpec target)
        => Emitters.SelectMany(e => e.GetAttributesToApply(target));

    /// <summary>
    /// Allows to specify additional interfaces to implements based on the target <see cref="TypeGenerationSpec"/>.
    /// </summary>
    /// <param name="target">The target <see cref="TypeGenerationSpec"/>.</param>
    /// <returns>A list of the additional additional interfaces to implement.</returns>
    protected internal virtual IEnumerable<string> GetTargetInterfacesToImplement(TypeGenerationSpec target)
        => Emitters.SelectMany(e => e.GetInterfacesToImplement(target));

    /// <inheritdoc />
    public override IEnumerable<SourceCodeEmitter<TypeGenerationSpec>> GetSourceCodeEmitters() => Emitters;

    /// <inheritdoc/>
    public override string GetFileName(TypeGenerationSpec target) => $"{target.TargetType.Name}.g.cs";

    /// <inheritdoc/>
    public override void EmitTargetSourceCode(TypeGenerationSpec target, SourceWriter writer)
    {
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

            writer.WriteLine();
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

        string targetDeclaration = specClasses[0];
        int baseTargetDeclarationIndex = targetDeclaration.AsSpan().IndexOf(':');
        string? baseType = baseTargetDeclarationIndex == -1 ? Options.DefaultBaseType : null;

        bool hasBaseType = !string.IsNullOrWhiteSpace(baseType);
        bool hasInterfaces = !string.IsNullOrWhiteSpace(interfacesToImplement);
        string baseTypeWithInterfaces = !string.IsNullOrWhiteSpace(baseType) || !string.IsNullOrWhiteSpace(interfacesToImplement)
            // review: must check if base type implemented or any interface matches already implemented bases
            ? SeparatorOrEmpty(baseTargetDeclarationIndex != -1, " : ")
                + (baseType ?? string.Empty) 
                + SeparatorOrEmpty(hasBaseType && hasInterfaces, CommaWithSpace)
                + interfacesToImplement
            : string.Empty;

        // Emit the target class declaration with it's body.
        writer.WriteLine($"{targetDeclaration}{baseTypeWithInterfaces}");
        writer.OpenBlock();
        base.EmitTargetSourceCode(target, writer);
        writer.CloseBlock();

        static string SeparatorOrEmpty(bool hasBaseTypeAndInterface, string separator) => hasBaseTypeAndInterface ? separator : string.Empty;
    }
}