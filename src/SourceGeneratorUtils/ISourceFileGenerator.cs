namespace SourceGeneratorUtils;

/// <summary>
/// Generates a <see cref="SourceFileDescriptor"/> for the given target type.
/// </summary>
/// <typeparam name="TDescriptor">The type of the target descriptor to generate a source file for.</typeparam>
public interface ISourceFileGenerator<TDescriptor>
{
    /// <summary>
    /// Generates a ready-to-compile <see cref="SourceFileDescriptor"/> for the given target type.
    /// </summary>
    /// <param name="target">The target to generate a source file for.</param>
    /// <param name="relatives">The other types being generated.</param>
    /// <returns></returns>
    SourceFileDescriptor GenerateSource(in TDescriptor target, IReadOnlyDictionary<string, TDescriptor>? relatives = null);
}