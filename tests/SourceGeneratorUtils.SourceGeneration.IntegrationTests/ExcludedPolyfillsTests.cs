using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SourceGeneratorUtils.SourceGeneration.IntegrationTests;

public class ExcludedPolyfillsTests
{
    private static readonly Assembly _thisAssembly = typeof(ExcludedPolyfillsTests).Assembly;

    [Theory]
    [MemberData(nameof(GetExcludedPolyfills))]
    public void PolyfillShouldNotBeGenerated(Type excludedPolyfill)
        => NotEqual(excludedPolyfill.Assembly.FullName, _thisAssembly.FullName);

    /// <summary>
    /// Returns the polyfills that should not be generated since already defined within the .NET7 runtime.
    /// </summary>
    public static IEnumerable<object[]> GetExcludedPolyfills()
    {
        yield return new object[] { typeof(Index) };
        yield return new object[] { typeof(Range) };
        yield return new object[] { typeof(IsExternalInit) };
        yield return new object[] { typeof(NotNullWhenAttribute) };
        yield return new object[] { typeof(DoesNotReturnAttribute) };
        yield return new object[] { typeof(RequiredMemberAttribute) };
        yield return new object[] { typeof(NotNullIfNotNullAttribute) };
        yield return new object[] { typeof(SetsRequiredMembersAttribute) };
        yield return new object[] { typeof(CompilerFeatureRequiredAttribute) };
    }
}