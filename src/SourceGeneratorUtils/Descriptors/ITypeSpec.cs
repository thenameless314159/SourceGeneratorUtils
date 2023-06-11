namespace SourceGeneratorUtils;

public interface ITypeSpec
{
    string Name { get; }
    string Type { get; }
    string Namespace { get; }
    string? BaseTypeName { get; }
    string? Accessibility { get; }
    IList<ITypeSpec> ContainingTypes { get; }
    IList<ITypeSpec> BaseTypesAndThis { get; }
}