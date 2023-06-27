using System.Text;

namespace SourceGeneratorUtils;

/// <summary>
/// Provides extension methods for <see cref="StringBuilder"/>.
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Appends the specified span to the string builder.
    /// </summary>
    /// <param name="builder">The string builder.</param>
    /// <param name="span">The span to append.</param>
    /// <returns>The string builder.</returns>
    public static unsafe StringBuilder AppendSpan(this StringBuilder builder, ReadOnlySpan<char> span)
    {
        fixed (char* ptr = span)
        {
            builder.Append(ptr, span.Length);
        }

        return builder;
    }
}