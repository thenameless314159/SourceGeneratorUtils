using System.Reflection;

namespace SourceGeneratorUtils;

/// <summary>
/// Represents the options to use when emitting C# source file within the <see cref="TypeSourceFileEmitter"/> abstraction.
/// </summary>
public record TypeSourceFileEmitterOptions : SourceFileEmitterOptions
{
    /// <summary>
    /// The default <see cref="TypeSourceFileEmitterOptions"/> instance.
    /// </summary>
    public new static readonly TypeSourceFileEmitterOptions Default = new();

    /// <summary>
    /// Specifies the assembly name of the source file emitter. This will be used to add the
    /// <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/> to the emitted source file target type.
    /// To disable the addition of the attribute, set this property to null.
    /// </summary>
    public AssemblyName? AssemblyName { get; init; }

    /// <summary>
    /// The default base type the target inherit from. If left to null, no base type will be inherited.
    /// </summary>
    public string? DefaultBaseType { get; init; }

    /// <summary>
    /// Whether to use combined attribute declaration.
    /// If set to true, multiple attributes will be declared in the same bracket
    /// like the following: <code>[Attribute1, Attribute2, Attribute3]</code>.
    /// Otherwise, each attribute will be declared separately in its own brackets.
    /// </summary>
    public bool UseCombinedAttributeDeclaration { get; init; }

    /// <summary>
    /// The default attributes that will be applied to the target generated type.
    /// </summary>
    public IReadOnlyList<string> DefaultAttributes { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The default interfaces that must be implemented by the target generated type.
    /// </summary>
    public IReadOnlyList<string> DefaultInterfaces { get; init; } = Array.Empty<string>();
}