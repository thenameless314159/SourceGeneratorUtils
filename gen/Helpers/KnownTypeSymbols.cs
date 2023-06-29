using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorUtils.SourceGeneration;

public sealed class KnownTypeSymbols
{
    public Compilation Compilation { get; }

    public KnownTypeSymbols(Compilation compilation) => Compilation = compilation;
}
    