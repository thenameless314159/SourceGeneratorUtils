namespace SourceGeneratorUtils;

/// <summary>
/// Represents the options to use when generating a c# source file.
/// </summary>
public record SourceFileGenOptions
{
    /// <summary>
    /// Whether to use combined attributes. If true, attributes will be combined into one attribute
    /// and will looks like this: <code>[Attribute1, Attribute2, Attribute3]</code>.
    /// </summary>
    public bool UseCombinedAttributes { get; init; }

    /// <summary>
    /// Whether to use file-scoped namespace. If true, the namespace will be declared like this:
    /// <code>namespace MyNamespace.MySubDomain;</code>
    /// </summary>
    public bool UseFileScopedNamespace { get; init; }

    /// <summary>
    /// Whether to use nullable warnings. If true, nullable warnings will be enabled otherwise disabled.
    /// </summary>
    /// <remarks>The nullable directive is added to the top of the file anyway.</remarks>
    public bool EnableNullableWarnings { get; init; }

    /// <summary>
    /// Whether to use nullable annotations. If true, nullable annotations will be enabled otherwise disabled.
    /// </summary>
    /// <remarks>The nullable directive is added to the top of the file anyway.</remarks>
    public bool EnableNullableAnnotations { get; init; } = true;

    /// <summary>
    /// The writer factory to use to create a new <see cref="SourceWriter"/> instance.
    /// Leave null if you don't need to customize the writer.
    /// </summary>
    public Func<SourceWriter>? WriterFactory { get; init; } = null;

    /// <summary>
    /// The default base type that the target <see cref="ITypeSpec"/> will inherit from if the target base type isn't specified.
    /// </summary>
    public string? DefaultBaseType { get; init; }

    /// <summary>
    /// The default interfaces that will implement the target <see cref="ITypeSpec"/>.
    /// </summary>
    public IReadOnlyList<string> DefaultInterfaces { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The default attributes that will be applied to the target <see cref="ITypeSpec"/>.
    /// </summary>
    public IReadOnlyList<string> DefaultAttributes { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The default using directives that will be added to the top of the file.
    /// </summary>
    public IReadOnlyList<string> DefaultUsingDirectives { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The block generators that will be used to generate the target <see cref="ITypeSpec"/> body.
    /// </summary>
    public IReadOnlyList<ISourceBlockGenerator> BlockGenerators { get; init; } = Array.Empty<ISourceBlockGenerator>();
}