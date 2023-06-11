namespace SourceGeneratorUtils;

public interface ISourceFileGenerator<in TDescriptor>
{
    bool TryGenerateSource(TDescriptor target, SourceFileGenOptions options, out SourceFileDescriptor sourceFile);
}