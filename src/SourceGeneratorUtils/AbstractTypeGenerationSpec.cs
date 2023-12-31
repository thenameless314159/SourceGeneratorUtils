﻿namespace SourceGeneratorUtils;

/// <summary>
/// A basic abstraction with an <see cref="ImmutableEquatableArray{T}"/> of <see cref="string"/> that should contains type declarations in order
/// to be used within a <see cref="SourceFileEmitter{TSpec}"/>.
/// A default implementation with a target <see cref="ITypeDescriptor"/> is provided as <see cref="DefaultGenerationSpec"/>.
/// </summary>
/// <remarks>
/// For source generators, type needs to be cacheable as a Roslyn incremental value so it must be
///
/// 1) immutable and
/// 2) implement structural (pointwise) equality comparison.
///
/// We can get these properties for free provided that we
///
/// a) define the type as an immutable C# record and
/// b) ensure all nested members are also immutable and implement structural equality.
///
/// When implementing this abstraction and adding new members to the type, please ensure that these properties
/// are satisfied otherwise you risk breaking incremental caching in your source generator!
/// </remarks>
public abstract record AbstractTypeGenerationSpec : AbstractGenerationSpec
{
    /// <summary>
    /// Gets a list of type declarations to be used while generating the target type declaration.
    /// </summary>
    /// <remarks>
    /// The first item in the array (at index 0) must be the target type declaration and may contains its xml documentation.
    /// </remarks>
    public required ImmutableEquatableArray<string> TypeDeclarations { get; init; }
}