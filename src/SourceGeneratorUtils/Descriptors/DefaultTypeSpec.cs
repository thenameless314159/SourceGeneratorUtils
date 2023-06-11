namespace SourceGeneratorUtils.Descriptors;

public sealed record DefaultTypeSpec
    (string Name, string Type, string Namespace, string? BaseTypeName = null, string? Accessibility = null) : ITypeSpec
{
    public IList<string> Attributes { get; init; } = Array.Empty<string>();
    public IList<ITypeSpec> ContainingTypes { get; init; } = Array.Empty<ITypeSpec>();
}