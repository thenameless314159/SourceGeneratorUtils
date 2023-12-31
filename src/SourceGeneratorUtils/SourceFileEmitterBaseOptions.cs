﻿namespace SourceGeneratorUtils;

/// <summary>
/// Represents the options to use when emitting C# source file within the <see cref="SourceFileEmitterBase{TSpec}"/> abstraction.
/// </summary>
public record SourceFileEmitterBaseOptions
{
    /// <summary>
    /// The default <see cref="SourceFileEmitterBaseOptions"/> instance.
    /// </summary>
    public static readonly SourceFileEmitterBaseOptions Default = new();

    /// <summary>
    /// The header to add to the top of the file. If left to null, use the <see cref="WellKnownStrings.ShortSourcePrefix"/> value by default.
    /// </summary>
    public string? SourceFileHeader { get; init; }

    /// <summary>
    /// Determines whether to use nullable reference type annotations.
    /// If set to true, the nullable reference types feature will be enabled, otherwise disabled.
    /// </summary>
    /// <remarks>
    /// The `#nullable enable` or `#nullable disable` directive will be added anyway to the top of the file based on the value of this property.
    /// </remarks>
    public bool EnableNullableWarnings { get; init; }

    /// <summary>
    /// Determines whether to use nullable reference type annotations.
    /// If set to true, the nullable reference types feature will be enabled, otherwise disabled.
    /// </summary>
    /// <remarks>
    /// The `#nullable enable` or `#nullable disable` directive will be added anyway to the top of the file based on the value of this property.
    /// </remarks>
    public bool EnableNullableAnnotations { get; init; } = true;

    /// <summary>
    /// Determines whether to use file-scoped namespace declarations.
    /// If set to true, the namespace will be declared in file-scoped manner like the following:
    /// <code>namespace MyNamespace.MySubDomain;</code>
    /// </summary>
    /// <remarks>
    /// This feature was introduced in C# 10.0 to improve readability by reducing indentation.
    /// Important: Please make sure that the target assembly to emit source file to is using C# 10.0 or above.
    /// </remarks>
    public bool UseFileScopedNamespace { get; init; }

    /// <summary>
    /// The number of blank lines to add between each declarations produced by the <see cref="SourceFileEmitterBase{TSpec}.CreateSourceWriter"/> method.
    /// </summary>
    public int BlankLinesBetweenDeclarations { get; init; } = 1;

    /// <summary>
    /// The analyzers warnings to suppress.
    /// If any, a <code>#pragma warnings disable</code> directive will be added to the top of the file
    /// with the configured warnings to suppress.
    /// </summary>
    public IReadOnlyList<string> SuppressWarnings { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The default using directives to write outside the namespace declaration of the emitted source file.
    /// </summary>
    public IReadOnlyList<string> DefaultOuterUsingDirectives { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The default using directives to write inside the namespace declaration of the emitted source file.
    /// </summary>
    public IReadOnlyList<string> DefaultInnerUsingDirectives { get; init; } = Array.Empty<string>();
}