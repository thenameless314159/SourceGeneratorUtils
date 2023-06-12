﻿namespace SourceGeneratorUtils;

/// <summary>
/// Represents a C# source file generator using <see cref="ITypeSpec"/> as target descriptor.
/// </summary>
public class CSharpSourceFileGenerator : CSharpSourceFileGenerator<ITypeSpec>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CSharpSourceFileGenerator"/> class.
    /// </summary>
    /// <param name="options">The <see cref="SourceFileGenOptions"/> to use.</param>
    public CSharpSourceFileGenerator(SourceFileGenOptions options) : base(options)
    {
    }
}

/// <summary>
/// Represents a C# source file generator for the given <see cref="ITypeSpec"/> implementation.
/// </summary>
/// <typeparam name="TDescriptor">he type of the target descriptor to generate a source file for.</typeparam>
public class CSharpSourceFileGenerator<TDescriptor> : ISourceFileGenerator<TDescriptor> where TDescriptor : ITypeSpec
{
    private readonly SourceFileGenOptions _options;

    /// <summary>
    /// Initializes a new instance of the strongly-typed <see cref="CSharpSourceFileGenerator{T}"/> class.
    /// </summary>
    /// <param name="options">The <see cref="SourceFileGenOptions"/> to use.</param>
    public CSharpSourceFileGenerator(SourceFileGenOptions options) => _options = options;

    /// <summary>
    /// Gets the file name for the given target descriptor.
    /// </summary>
    /// <param name="target">The target <see cref="ITypeSpec"/>.</param>
    /// <returns>The file name of the target <see cref="ITypeSpec"/>.</returns>
    protected virtual string GetFileName(in TDescriptor target) => $"{target.Name}.g.cs";

    /// <inheritdoc />
    public SourceFileDescriptor GenerateSource(in TDescriptor target, IReadOnlyDictionary<string, TDescriptor>? descriptors = null)
    {
        const string publicKeyword = "public", commaSeparator = ", ", enable = "enable", disable = "disable";

        var context = SourceWritingContext.CreateFor(target, _options, descriptors);
        var writer = _options.WriterFactory?.Invoke() ?? new SourceWriter();

        // Nullable annotations and warnings declaration on top of the file
        writer.WriteLine($"#nullable {GetEnabledString(_options.EnableNullableAnnotations)} annotations");
        writer.WriteLine($"#nullable {GetEnabledString(_options.EnableNullableWarnings)} warnings");
        writer.WriteLine();

        // Using directives declaration
        var namespacesToImport = _options.BlockGenerators
            .SelectMany(c => c.GetImportedNamespaces(context))
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
        var implementedInterfaces = _options.BlockGenerators
            .SelectMany(c => c.GetImplementedInterfaces(context))
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
        for (int i = 0; i < _options.BlockGenerators.Count; i++)
        {
            int writerLengthBefore = writer.Length; // store the length before writing the block to determine if something was written
            _options.BlockGenerators[i].GenerateBlock(writer, context);

            // continue if we reached the end or if nothing was written
            if (writerLengthBefore == writer.Length 
                || i == _options.BlockGenerators.Count - 1)
                continue;

            writer.WriteLine(); // otherwise add a space between blocks
        }

        return new SourceFileDescriptor(GetFileName(in target), writer.CloseAllBlocks());
        static string GetEnabledString(bool enabled) => enabled ? enable : disable;
        static string GetSeparatorOrEmpty(bool hasBaseTypeAndInterface) => hasBaseTypeAndInterface ? commaSeparator : string.Empty;
    }
}