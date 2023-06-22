using static SourceGeneratorUtils.WellKnownStrings;
using static SourceGeneratorUtils.WellKnownChars;
using System.Text;

namespace SourceGeneratorUtils;

/// <summary>
/// Provides extension methods for <see cref="TypeDesc"/>.
/// </summary>
public static class TypeDescExtensions
{
    /// <summary>
    /// Gets the type declaration for the given <paramref name="descriptor"/> followed by its containing types declarations.
    /// </summary>
    /// <param name="descriptor">The type descriptor.</param>
    /// <returns>An enumerable with the target type declaration followed by the containing types declarations.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static IEnumerable<string> GetTypeDeclarationWithContainingTypes(this TypeDesc descriptor)
    {
        yield return descriptor.ToTypeDeclaration();

        foreach (var containingType in descriptor.ContainingTypes)
            yield return containingType.ToTypeDeclaration();
    }

    /// <summary>
    /// Gets the type name declaration for the given <paramref name="descriptor"/> with generic types if any.
    /// </summary>
    /// <param name="descriptor">The type descriptor.</param>
    /// <returns>The type name with generic types if any.</returns>
    public static string GetGenericTypeNameDeclaration(this TypeDesc descriptor)
    {
        if (descriptor.GenericTypes.Count == 0)
            return descriptor.Name;

        StringBuilder sb = new();
        AppendTypeNameDeclaration(descriptor, sb);
        return sb.ToString();
    }

    /// <summary>
    /// Gets the type modifiers declaration for the given <paramref name="descriptor"/> or an empty string if none.
    /// </summary>
    /// <param name="descriptor">The type descriptor.</param>
    /// <returns>The type modifiers declaration or an empty string if none.</returns>
    public static string GetTypeModifiersDeclaration(this TypeDesc descriptor)
    {
        StringBuilder sb = new();
        AppendTypeModifiers(descriptor, sb);
        return sb.ToString();
    }

    /// <summary>
    /// Converts the given <paramref name="descriptor"/> to a <see cref="TypeRef"/>.
    /// </summary>
    /// <param name="descriptor">The type descriptor.</param>
    /// <returns>The type name with generic types if any.</returns>
    public static TypeRef ToTypeRef(this TypeDesc descriptor)
        => new
        (
            descriptor.Name,
            descriptor.Namespace,
            descriptor.TypeKind,
            descriptor.IsValueType,
            descriptor.SpecialType
        );

    /// <summary>
    /// Converts the given <paramref name="descriptor"/> to a type declaration like <c>public class MyClass<T1></T1></c>.
    /// </summary>
    /// <param name="descriptor">The type descriptor.</param>
    /// <returns>A formatted type declaration.</returns>
    public static string ToTypeDeclaration(this TypeDesc descriptor)
    {
        StringBuilder sb = new();

        sb.Append(descriptor.Accessibility.GetAccessibilityString());
        sb.Append(' ');

        AppendTypeModifiers(descriptor, sb);

        sb.Append(descriptor.TypeKind.GetTypeKindString(descriptor.IsRecord));
        sb.Append(' ');

        AppendTypeNameDeclaration(descriptor, sb);

        bool hasBaseType = descriptor.BaseTypes.Count > 0;
        bool hasInterfaces = descriptor.Interfaces.Count > 0;

        if (hasBaseType || hasInterfaces)
            sb.Append(" : ");

        if (hasBaseType)
            AppendTypeNameDeclaration(descriptor.BaseTypes[0], sb);

        if (!hasInterfaces) 
            return sb.ToString();

        if (hasBaseType)
            sb.Append(CommaWithSpace);

        for (int i = 0; i < descriptor.Interfaces.Count; i++)
        {
            AppendTypeNameDeclaration(descriptor.Interfaces[i], sb);

            if (i < descriptor.Interfaces.Count - 1)
            {
                sb.Append(CommaWithSpace);
            }
        }

        return sb.ToString();
    }

    private static void AppendTypeModifiers(TypeDesc descriptor, StringBuilder sb)
    {
        if (descriptor is { IsStatic: true, IsValueType: false, IsSealed: false })
        {
            sb.Append("static ");
        }
        if (descriptor is { IsSealed: true, IsValueType: false, IsStatic: false })
        {
            sb.Append("sealed ");
        }
        if (descriptor is { IsAbstract: true, IsValueType: false, IsStatic: false })
        {
            sb.Append("abstract ");
        }
        if (descriptor is { IsReadOnly: true, IsValueType: true })
        {
            sb.Append("readonly ");
        }
        if (descriptor.IsPartial)
        {
            sb.Append("partial ");
        }
    }

    private static void AppendTypeNameDeclaration(TypeDesc descriptor, StringBuilder sb)
    {
        sb.Append(descriptor.Name);

        if (descriptor.GenericTypes.Count > 0)
        {
            sb.Append(OpenAngle);
            AppendGenericTypes(descriptor.GenericTypes, sb);
            sb.Append(CloseAngle);
        }

        // review: might need a rework for a non-recursive approach instead
        static void AppendGenericTypes(IReadOnlyList<TypeDesc> genericTypes, StringBuilder sb)
        {
            for (int i = 0; i < genericTypes.Count; i++)
            {
                TypeDesc genericType = genericTypes[i];
                sb.Append(genericType.Name);

                if (genericType.GenericTypes.Count > 0)
                {
                    sb.Append(OpenAngle);
                    AppendGenericTypes(genericType.GenericTypes, sb);
                    sb.Append(CloseAngle);
                }

                if (i < genericTypes.Count - 1)
                {
                    sb.Append(CommaWithSpace);
                }
            }
        }
    }
}