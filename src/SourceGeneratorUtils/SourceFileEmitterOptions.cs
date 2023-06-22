using System.Reflection;

namespace SourceGeneratorUtils;

/// <summary>
/// Represents the options to use when emitting C# source file within the <see cref="SourceFileEmitter{TSpec}"/> abstraction.
/// </summary>
public record SourceFileEmitterOptions : SourceFileEmitterBaseOptions
{
    /// <summary>
    /// The default <see cref="SourceFileEmitterOptions"/> instance.
    /// </summary>
    public new static readonly SourceFileEmitterOptions Default = new();

    /// <summary>
    /// The number of blank lines to add between each configured <see cref="SourceCodeEmitter{TSpec}.EmitTargetSourceCode"/> call.
    /// </summary>
    public int BlankLinesBetweenCodeEmitters { get; init; } = 1;

    /// <summary>
    /// Specifies the assembly name of the source file emitter. This will be used to add the
    /// <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/> to the emitted source file target type.
    /// To disable the addition of the attribute, set this property to null.
    /// </summary>
    public AssemblyName? AssemblyName { get; init; }

    /// <summary>
    /// The default base type that the target <see cref="AbstractTypeGenerationSpec.TypeDeclarations"/> must inherit from.
    /// If left to null, no base type will be inherited.
    /// </summary>
    public string? DefaultBaseType { get; init; }

    /// <summary>
    /// Whether to inject the configured <see cref="SourceFileEmitter{TSpec}.Options"/> instance
    /// to the configured <see cref="SourceCodeEmitter{TSpec}"/> instances.
    /// </summary>
    public bool InjectOptionsOnCodeEmitters { get; init; } = true;

    /// <summary>
    /// Whether to use combined attribute declaration.
    /// If set to true, multiple attributes will be declared in the same bracket
    /// like the following: <code>[Attribute1, Attribute2, Attribute3]</code>.
    /// Otherwise, each attribute will be declared separately in its own brackets.
    /// </summary>
    public bool UseCombinedAttributeDeclaration { get; init; }
    
    /// <summary>
    /// The default attributes that will be applied to the target generated <see cref="AbstractTypeGenerationSpec.TypeDeclarations"/>.
    /// </summary>
    public IReadOnlyList<string> DefaultAttributes { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The default interfaces that must be implemented by the target generated <see cref="AbstractTypeGenerationSpec.TypeDeclarations"/>.
    /// </summary>
    public IReadOnlyList<string> DefaultInterfaces { get; init; } = Array.Empty<string>();
}