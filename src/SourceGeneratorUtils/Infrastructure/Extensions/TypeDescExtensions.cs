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
    /// Converts the given <paramref name="descriptor"/> to a type declaration like <c>public class MyClass<T1></T1></c>.
    /// </summary>
    /// <param name="descriptor">The type descriptor.</param>
    /// <returns>A formatted type declaration.</returns>
    public static string ToTypeDeclaration(this TypeDesc descriptor)
    {
        StringBuilder sb = new();

        string accessibility = descriptor.Accessibility.GetAccessibilityString();
        if (!string.IsNullOrEmpty(descriptor.TypeModifier))
        {
            sb.Append(accessibility);
            sb.Append(' ');
        }

        if (!string.IsNullOrEmpty(descriptor.TypeModifier))
        {
            sb.Append(descriptor.TypeModifier);
            sb.Append(' ');
        }

        sb.Append(descriptor.TypeKind.GetTypeKindString(descriptor.IsRecord));
        sb.Append(' ');

        sb.Append(descriptor.Name);

        if (descriptor.GenericTypes.Count > 0)
            AppendGenericTypes(descriptor.GenericTypes, sb);

        return sb.ToString(); // review: append base types and interfaces as well ? may not be needed

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