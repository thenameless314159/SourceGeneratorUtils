using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.Tests;

public class TypeDescExtensionsTests
{
    [Theory]
    [InlineData(Accessibility.NotApplicable)]
    [InlineData(Accessibility.ProtectedOrInternal)]
    public void ToTypeDeclaration_ThrowsOnInvalidAccessibility(Accessibility accessibility)
        => Throws<ArgumentOutOfRangeException>(() => 
            Create("TestClass", accessibility: accessibility)
                .ToTypeDeclaration());

    [Theory]
    [MemberData(nameof(GetInvalidTypeKinds))]
    public void ToTypeDeclaration_ThrowsOnInvalidTypeKind(TypeKind typeKind)
        => Throws<ArgumentOutOfRangeException>(() =>
            Create("TestClass", kind: typeKind)
                .ToTypeDeclaration());

    [Fact]
    public void GetGenericTypeDeclaration_ReturnsTypeNameWithGenerics()
    {
        var typeNameDeclaration = _genericTypeDesc.GetGenericTypeNameDeclaration();

        const string expected = "Generic<T1, T2<T3>, T4<T5<T6>>>";
        Equal(expected, typeNameDeclaration);
    }

    [Fact]
    public void GetGenericTypeDeclaration_ReturnsSelfWhenNoGenericTypes()
    {
        var typeDesc = Create("TestClass");
        Equal(typeDesc.Name, typeDesc.GetGenericTypeNameDeclaration());
    }

    [Theory]
    [InlineData(Accessibility.Public)]
    [InlineData(Accessibility.Private)]
    [InlineData(Accessibility.ProtectedAndInternal)]
    public void ToTypeDeclaration_ShouldStartsWithAccessibility(Accessibility accessibility)
    {
        var typeDeclaration = Create("TestClass", accessibility: accessibility).ToTypeDeclaration();
        StartsWith(accessibility.GetAccessibilityString(), typeDeclaration);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("sealed")]
    [InlineData("partial sealed")]
    public void ToTypeDeclaration_ShouldIncludeOptionalModifiers(string? modifiers)
    {
        var typeDeclaration = Create("TestClass", modifier: modifiers).ToTypeDeclaration();
        if (modifiers is not null) Contains(modifiers, typeDeclaration);
        else Equal("public class TestClass", typeDeclaration);
    }

    [Theory]
    [InlineData(TypeKind.Class, true)]
    [InlineData(TypeKind.Class, false)]
    [InlineData(TypeKind.Struct, true)]
    [InlineData(TypeKind.Struct, false)]
    public void ToTypeDeclaration_ShouldIncludeTypeKind(TypeKind typeKind, bool isRecord)
    {
        var typeDeclaration = Create("TestType", kind: typeKind, isRecord: isRecord).ToTypeDeclaration();

        string expected = $"public {typeKind.GetTypeKindString(isRecord)} TestType";
        Equal(expected, typeDeclaration);
    }

    [Fact]
    public void ToTypeDeclaration_ShouldIncludeGenericTypes()
    {
        var typeDeclaration = _genericTypeDesc.ToTypeDeclaration();

        const string expected = "public class Generic<T1, T2<T3>, T4<T5<T6>>>";
        Equal(expected, typeDeclaration);
    }

    [Fact]
    public void ToTypeDeclaration_ShouldIncludeBaseType()
    {
        var typeDeclaration = Create("ClassWithBase", baseType: Create("BaseType")).ToTypeDeclaration();

        const string expected = "public class ClassWithBase : BaseType";
        Equal(expected, typeDeclaration);
    }

    [Fact]
    public void ToTypeDeclaration_ShouldIncludeInterfaces()
    {
        var typeDeclaration = Create("ClassWithInterfaces", interfaces: new []
        {
            Create("IInterface"),
            Create("IGeneric<int>"),
            Create("IGeneric", genericTypes: new []{ Create("long") }),
        }).ToTypeDeclaration();

        const string expected = "public class ClassWithInterfaces : IInterface, IGeneric<int>, IGeneric<long>";
        Equal(expected, typeDeclaration);
    }

    [Fact]
    public void ToTypeDeclaration_ShouldIncludeBaseTypeWithInterfaces()
    {
        var typeDeclaration = Create("ClassWithBoth", baseType: Create("BaseType"), interfaces: new[]
        {
            Create("IInterface"),
            Create("IGeneric<int>"),
            Create("IGeneric", genericTypes: new []{ Create("long") }),
        }).ToTypeDeclaration();

        const string expected = "public class ClassWithBoth : BaseType, IInterface, IGeneric<int>, IGeneric<long>";
        Equal(expected, typeDeclaration);
    }

    [Fact]
    public void GetTypeDeclarationWithContainingTypes_ReturnsTargetWithContainingTypesDeclarations()
    {
        var typeDesc = Create("TestClass", containingTypes: new[] { Create("Containing1"), Create("Containing2") });
        var typeDeclarations = typeDesc.GetTypeDeclarationWithContainingTypes();

        var expected = new[] { typeDesc.ToTypeDeclaration() }
            .Concat(typeDesc.ContainingTypes
                .Select(static t => t.ToTypeDeclaration()));

        Equal(expected, typeDeclarations);
    }

    private static TypeDesc Create(string name, 
        string? ns = null, 
        string? modifier = null, 
        bool isValueType = false, 
        bool isRecord = false,
        TypeDesc? baseType = null,
        TypeKind kind = TypeKind.Class, 
        SpecialType specialType = SpecialType.None,
        Accessibility accessibility = Accessibility.Public,
        IEnumerable<TypeDesc>? interfaces = null,
        IEnumerable<TypeDesc>? genericTypes = null,
        IEnumerable<TypeDesc>? containingTypes = null
        )
        => new()
        {
            Name = name,
            Namespace = ns,
            TypeKind = kind,
            IsRecord = isRecord,
            TypeModifier = modifier,
            IsValueType = isValueType,
            SpecialType = specialType,
            Accessibility = accessibility,
            Interfaces = new ImmutableEquatableArray<TypeDesc>(interfaces ?? Array.Empty<TypeDesc>()),
            GenericTypes = new ImmutableEquatableArray<TypeDesc>(genericTypes ?? Array.Empty<TypeDesc>()),
            ContainingTypes = new ImmutableEquatableArray<TypeDesc>(containingTypes ?? Array.Empty<TypeDesc>()),
            BaseTypes = baseType is not null ? ImmutableEquatableArray.Create(baseType) : ImmutableEquatableArray<TypeDesc>.Empty,
        };

    private static readonly TypeDesc _genericTypeDesc = Create("Generic", genericTypes: new[]
    {
        Create("T1"),
        Create("T2", genericTypes: new []{ Create("T3")}),
        Create("T4", genericTypes: new []{ Create("T5", genericTypes : new[] { Create("T6") }) }),
    });

    public static IEnumerable<object[]> GetInvalidTypeKinds()
    {
        yield return new object[] { TypeKind.Array };
        yield return new object[] { TypeKind.Delegate };
        yield return new object[] { TypeKind.Dynamic };
        yield return new object[] { TypeKind.Error };
        yield return new object[] { TypeKind.Module };
        yield return new object[] { TypeKind.Pointer };
        yield return new object[] { TypeKind.Submission };
        yield return new object[] { TypeKind.TypeParameter };
        yield return new object[] { TypeKind.FunctionPointer };
    }
}