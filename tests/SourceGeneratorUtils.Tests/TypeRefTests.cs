using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.Tests;

public class TypeRefTests
{
    [Theory]
    [InlineData("TestClass", null)]
    [InlineData("TestClass", "SourceGeneratorUtils.Tests")]
    [InlineData("TypeModel", "SourceGeneratorUtils.Models")]
    public void Ctor_WithTypeAndNamespaceParameters_ReturnsInstanceWithFullyQualifiedName(string name, string? ns)
    {
        var typeRef = new TypeRef(name, ns, TypeKind.Class);

        True(typeRef.CanBeNull);
        Equal(name, typeRef.Name);
        False(typeRef.IsValueType);
        Equal(TypeKind.Class, typeRef.TypeKind);
        Equal(SpecialType.None, typeRef.SpecialType);

        string expected = ns is null ? $"global::{name}" : $"global::{ns}.{name}";
        Equal(expected, typeRef.FullyQualifiedName);
    }
}