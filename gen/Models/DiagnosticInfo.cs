using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.SourceGeneration;

/// <summary>
/// Descriptor for diagnostic instances using structural equality comparison.
/// Provides a work-around for https://github.com/dotnet/roslyn/issues/68291.
/// </summary>
internal readonly struct DiagnosticInfo : IEquatable<DiagnosticInfo>
{
    public required DiagnosticDescriptor Descriptor { get; init; }
    public required object?[] MessageArgs { get; init; }
    public required Location? Location { get; init; }

    public Diagnostic CreateDiagnostic()
        => Diagnostic.Create(Descriptor, Location, MessageArgs);

    public readonly override bool Equals(object? obj) 
        => obj is DiagnosticInfo info && Equals(info);

    public readonly bool Equals(DiagnosticInfo other) 
        => Descriptor.Equals(other.Descriptor) && 
            MessageArgs.SequenceEqual(other.MessageArgs) && 
            Location == other.Location;

    public readonly override int GetHashCode()
    {
        int hashCode = Descriptor.GetHashCode();
        foreach (object? messageArg in MessageArgs)
        {
            hashCode = HashHelpers.Combine(hashCode, messageArg?.GetHashCode() ?? 0);
        }

        hashCode = HashHelpers.Combine(hashCode, Location?.GetHashCode() ?? 0);
        return hashCode;
    }
}

file static class HashHelpers
{
    public static int Combine(int h1, int h2) => (h1 << 5 | h1 >>> 27) + h1 ^ h2;
}