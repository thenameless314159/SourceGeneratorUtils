namespace SourceGeneratorUtils;

/// <summary>
/// Generates a <see cref="SourceFile"/> for the given target type.
/// </summary>
/// <typeparam name="TDescriptor">The type of the target descriptor to generate a source file for.</typeparam>
public interface ISourceFileGenerator<in TDescriptor>
{
    /// <summary>
    /// Generates a ready-to-compile <see cref="SourceFile"/> for the given target type.
    /// </summary>
    /// <param name="target">The target to generate a source file for.</param>
    /// <returns>A ready-to-compile <see cref="SourceFile"/> for the given target type.</returns>
    SourceFile GenerateSource(TDescriptor target);
}