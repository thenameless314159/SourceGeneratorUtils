namespace SourceGeneratorUtils;

/// <summary>
/// Represents a C# source file generator for a given <see cref="ITypeSpec"/> implementation.
/// </summary>
/// <typeparam name="TDescriptor">The type of the target descriptor to generate a source file for.</typeparam>
public class CSharpSourceGenerator<TDescriptor> : ISourceFileGenerator<TDescriptor> 
    where TDescriptor : ITypeSpec
{
    /// <summary>
    /// Gets the <see cref="CSharpSourceBlockWriter{TDescriptor}"/> instances to use within this generator.
    /// </summary>
    protected internal readonly IReadOnlyList<CSharpSourceBlockWriter<TDescriptor>> _blockWriters;

    /// <summary>
    /// Gets the <see cref="SourceFileGenOptions"/> to use within this generator.
    /// </summary>
    protected internal readonly SourceFileGenOptions _options;

    /// <summary>
    /// Creates a new <see cref="CSharpSourceGenerator{TDescriptor}"/> instance for the given <paramref name="options"/>
    /// and <paramref name="blockWriters"/>.
    /// </summary>
    /// <param name="options">The <see cref="SourceFileGenOptions"/> to use.</param>
    /// <param name="blockWriters">The <see cref="CSharpSourceBlockWriter{TDescriptor}"/> instances to use.</param>
    public CSharpSourceGenerator(SourceFileGenOptions options, IReadOnlyList<CSharpSourceBlockWriter<TDescriptor>> blockWriters)
        => (_options, _blockWriters) = (options, blockWriters);

    /// <summary>
    /// Creates a new <see cref="CSharpSourceGenerator{TDescriptor}"/> instance for the given <paramref name="options"/>
    /// and <paramref name="blockWriters"/>.
    /// </summary>
    /// <param name="options">The <see cref="SourceFileGenOptions"/> to use.</param>
    /// <param name="blockWriters">The <see cref="CSharpSourceBlockWriter{TDescriptor}"/> instances to use.</param>
    public CSharpSourceGenerator(SourceFileGenOptions options, params CSharpSourceBlockWriter<TDescriptor>[] blockWriters)
        => (_options, _blockWriters) = (options, blockWriters);

    /// <summary>
    /// Creates a new <see cref="CSharpSourceGenerator{TDescriptor}"/> instance for the given <paramref name="options"/>
    /// with no block writers.
    /// </summary>
    /// <param name="options">The <see cref="SourceFileGenOptions"/> to use.</param>
    public CSharpSourceGenerator(SourceFileGenOptions options) : this(options, Array.Empty<CSharpSourceBlockWriter<TDescriptor>>())
    {
    }

    /// <summary>
    /// Creates a new <see cref="CSharpSourceGenerator{TDescriptor}"/> instance with a default <see cref="SourceFileGenOptions"/>.
    /// </summary>
    public CSharpSourceGenerator() : this(SourceFileGenOptions.Default)
    {
    }

    /// <summary>
    /// Gets the file name for the given target descriptor.
    /// </summary>
    /// <param name="target">The target <see cref="ITypeSpec"/>.</param>
    /// <returns>The file name of the target <see cref="ITypeSpec"/>.</returns>
    protected virtual string GetFileName(TDescriptor target) => $"{target.Name}.g.cs";

    /// <inheritdoc />
    public SourceFileDescriptor GenerateSource(TDescriptor target)
    {
        const string publicKeyword = "public", commaSeparator = ", ", enable = "enable", disable = "disable";
        var writer = _options.WriterFactory?.Invoke() ?? new SourceWriter();

        // Nullable annotations and warnings declaration on top of the file
        writer.WriteLine($"#nullable {GetEnabledString(_options.EnableNullableAnnotations)} annotations");
        writer.WriteLine($"#nullable {GetEnabledString(_options.EnableNullableWarnings)} warnings");
        writer.WriteLine();

        // Using directives declaration
        var namespacesToImport = _blockWriters
            .SelectMany(c => c.GetImportedNamespaces(target, _options))
            .Concat(_options.DefaultUsingDirectives)
            .Distinct()
            .Select(StringHelpers.MakeUsingDirective)
            .OrderByDescending(ns => ns.Length);

        var usingDirectives = string.Join(Environment.NewLine, namespacesToImport);

        if (!string.IsNullOrWhiteSpace(usingDirectives))
            writer.WriteLine(usingDirectives).WriteLine();

        // Namespace declaration
        if (_options.UseFileScopedNamespace)
            writer.WriteLine($"namespace {target.Namespace};")
                .WriteLine();
        else
            writer.WriteLine($"namespace {target.Namespace}")
                .OpenBlock();

        // Containing types declaration
        if (target.ContainingTypes.Count > 0)
            foreach (var type in target.ContainingTypes)
            {
                writer.WriteLine($"{type.Accessibility ?? publicKeyword} {type.Type} {type.Name}");
                writer.OpenBlock();
            }

        // Target type attributes declaration
        var attributes = target.Attributes.Concat(_options.DefaultAttributes).Distinct();
        var attributesDeclaration = _options.UseCombinedAttributes
            ? $"[{string.Join(", ", attributes)}]"
            : string.Join(Environment.NewLine, attributes.Select(static a => $"[{a}]"));

        if (!string.IsNullOrWhiteSpace(attributesDeclaration))
            writer.WriteLine(attributesDeclaration).WriteLine();

        // Target type declaration
        var accessModifiers = target.Accessibility ?? publicKeyword;
        var implementedInterfaces = _blockWriters
            .SelectMany(c => c.GetImplementedInterfaces(target, _options))
            .Concat(_options.DefaultInterfaces)
            .Distinct();

        var (baseType, interfaces) = (target.BaseTypeName ?? _options.DefaultBaseType, string.Join(commaSeparator, implementedInterfaces));
        var (hasBaseType, hasInterfaces) = (!string.IsNullOrWhiteSpace(baseType), !string.IsNullOrWhiteSpace(interfaces));
        var baseTypeWithInterfaces = hasBaseType || hasInterfaces
            ? " : " + (baseType ?? string.Empty) + GetSeparatorOrEmpty(hasBaseType && hasInterfaces) + interfaces
            : string.Empty;

        // review: add options to support record declaration with semicolon instead ?
        //         then handle several types and group per namespace instead ?
        writer.WriteLine($"{accessModifiers} {target.Type} {target.Name}{baseTypeWithInterfaces}");
        writer.OpenBlock();

        // Type body, handled by block generators
        for (int i = 0; i < _blockWriters.Count; i++)
        {
            int writerLengthBefore = writer.Length; // store the length before writing the block to determine if something has been written
            _blockWriters[i].WriteTo(writer, target, _options);

            // continue if we reached the end or if nothing was written
            if (writerLengthBefore == writer.Length
                || i == _blockWriters.Count - 1)
                continue;

            writer.WriteLine(); // otherwise add a space between blocks
        }

        return new SourceFileDescriptor(GetFileName(target), writer.CloseAllBlocks());
        static string GetEnabledString(bool enabled) => enabled ? enable : disable;
        static string GetSeparatorOrEmpty(bool hasBaseTypeAndInterface) => hasBaseTypeAndInterface ? commaSeparator : string.Empty;
    }
}