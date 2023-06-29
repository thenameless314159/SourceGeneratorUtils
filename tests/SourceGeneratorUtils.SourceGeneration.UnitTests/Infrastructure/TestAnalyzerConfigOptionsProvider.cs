using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SourceGeneratorUtils.SourceGeneration.UnitTests;

public class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => TestAnalyzerConfigOptions.Empty;
    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => TestAnalyzerConfigOptions.Empty;

    public override AnalyzerConfigOptions GlobalOptions { get; }

    public TestAnalyzerConfigOptionsProvider(params (string, string)[] options)
    {
        GlobalOptions = new TestAnalyzerConfigOptions(options.ToDictionary(static t => t.Item1, static t => t.Item2));
    }

    public TestAnalyzerConfigOptionsProvider(Dictionary<string, string> options)
    {
        GlobalOptions = new TestAnalyzerConfigOptions(options);
    }

    private class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        public static readonly AnalyzerConfigOptions Empty = new TestAnalyzerConfigOptions(new Dictionary<string, string>());

        private readonly Dictionary<string, string> _options;

        public TestAnalyzerConfigOptions(Dictionary<string, string> options)
        {
            _options = options;
        }

        public override IEnumerable<string> Keys => _options.Keys;

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
            => _options.TryGetValue(key, out value);
    }
}