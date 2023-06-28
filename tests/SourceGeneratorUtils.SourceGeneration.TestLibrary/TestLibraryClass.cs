using System.Reflection;

namespace SourceGeneratorUtils.SourceGeneration;

public static class TestLibrary
{
    public static readonly Assembly Assembly = typeof(TestLibrary).Assembly;
}