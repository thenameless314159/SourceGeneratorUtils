using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SourceGeneratorUtils.SourceGeneration;

internal static class AnalyzerConfigOptionsProviderExtensions
{
    public static bool TryGetGlobalOptionsValue(this AnalyzerConfigOptionsProvider optionsProvider, string propertyName, 
        out bool propertyValue)
    {
        if (optionsProvider.GlobalOptions.TryGetValue(propertyName, out string? msBuildProperty))
        {
            propertyValue = string.Equals(msBuildProperty, bool.TrueString, StringComparison.InvariantCulture);
            return true;
        }

        propertyValue = false;
        return false;
    }

    public static bool TryGetGlobalOptionsValue(this AnalyzerConfigOptionsProvider optionsProvider, string propertyName,
        [NotNullWhen(true)] out ImmutableEquatableArray<string>? propertyValue)
    {
        if (optionsProvider.GlobalOptions.TryGetValue(propertyName, out string? msBuildProperty))
        {
            propertyValue = GetTypeNames(msBuildProperty);
            return true;
        }

        propertyValue = null;
        return false;

        static ImmutableEquatableArray<string> GetTypeNames(string value)
        {
            const char semicolon = ';', comma = ',';
            ReadOnlySpan<char> valueSpan = value.AsSpan();

            int indexOfSeparator = valueSpan.IndexOf(comma);
            char separator = indexOfSeparator == -1 ? semicolon : comma;

            // update the indexOfSeparator value with the new separator if necessary
            if (separator == semicolon) 
                indexOfSeparator = valueSpan.IndexOf(separator);

            return ImmutableEquatableArray.Create(indexOfSeparator != -1 ? value.Split(separator) : new[] { value });
        }
    }
}