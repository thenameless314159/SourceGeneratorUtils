namespace SourceGeneratorUtils;

/// <summary>
/// A basic abstractions that must be implemented with relevant members for your source generation logic
/// in order to models common specifications for generated source code.
/// This abstractions only contains an optional namespace member.
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
public abstract record AbstractGenerationSpec
{
    /// <summary>
    /// Gets or init the namespace of the generated source file.
    /// </summary>
    public string? Namespace { get; init; }
}