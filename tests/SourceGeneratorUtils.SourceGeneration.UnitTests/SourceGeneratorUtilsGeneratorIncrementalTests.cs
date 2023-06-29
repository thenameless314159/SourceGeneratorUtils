using Microsoft.CodeAnalysis;
using System.Collections;
using System.Reflection;

namespace SourceGeneratorUtils.SourceGeneration.UnitTests;

public static class SourceGeneratorUtilsGeneratorIncrementalTests
{
    [Theory]
    [MemberData(nameof(GetCompilationHelperFactories))]
    public static void CompilingTheSameSourceResultsInEqualModels(Func<Compilation> factory)
    {
        SourceGeneratorResult result1 = RunSourceGenerator(factory());
        SourceGeneratorResult result2 = RunSourceGenerator(factory());

        Equal(result1.SourceGenerationSpecs.Length, result2.SourceGenerationSpecs.Length);

        for (int i = 0; i < result1.SourceGenerationSpecs.Length; i++)
        {
            SourceGenerationSpec ctx1 = result1.SourceGenerationSpecs[i];
            SourceGenerationSpec ctx2 = result2.SourceGenerationSpecs[i];

            NotSame(ctx1, ctx2);
            AssertStructurallyEqual(ctx1, ctx2);

            Equal(ctx1, ctx2);
            Equal(ctx1.GetHashCode(), ctx2.GetHashCode());
        }
    }

    [Theory]
    [MemberData(nameof(GetCompilationHelperFactories))]
    public static void SourceGenModelDoesNotEncapsulateSymbolsOrCompilationData(Func<Compilation> factory)
    {
        var result = RunSourceGenerator(factory());
        WalkObjectGraph(result.SourceGenerationSpecs);

        static void WalkObjectGraph(object obj)
        {
            var visited = new HashSet<object>();
            Visit(obj);

            void Visit(object? node)
            {
                if (node is null || !visited.Add(node))
                {
                    return;
                }

                False(node is Compilation or ISymbol);

                Type type = node.GetType();
                if (type.IsPrimitive || type.IsEnum || type == typeof(string))
                {
                    return;
                }

                if (node is IEnumerable collection and not string)
                {
                    foreach (object? element in collection)
                    {
                        Visit(element);
                    }

                    return;
                }

                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    object? fieldValue = field.GetValue(node);
                    Visit(fieldValue);
                }
            }
        }
    }

    public static IEnumerable<object[]> GetCompilationHelperFactories()
        => typeof(CompilationHelper).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.ReturnType == typeof(Compilation) && m.GetParameters().Length == 0)
            .Select(m => new object[] { Delegate.CreateDelegate(typeof(Func<Compilation>), m) });

    /// <summary>
    /// Asserts for structural equality, returning a path to the mismatching data when not equal.
    /// </summary>
    private static void AssertStructurallyEqual<T>(T expected, T actual)
    {
        CheckAreEqualCore(expected, actual, new());
        static void CheckAreEqualCore(object? expected, object? actual, Stack<string> path)
        {
            if (expected is null || actual is null)
            {
                if (expected is not null || actual is not null)
                {
                    FailNotEqual();
                }

                return;
            }

            Type type = expected.GetType();
            if (type != actual.GetType())
            {
                FailNotEqual();
                return;
            }

            if (expected is IEnumerable leftCollection)
            {
                if (actual is not IEnumerable rightCollection)
                {
                    FailNotEqual();
                    return;
                }

                object?[] expectedValues = leftCollection.Cast<object?>().ToArray();
                object?[] actualValues = rightCollection.Cast<object?>().ToArray();

                for (int i = 0; i < Math.Max(expectedValues.Length, actualValues.Length); i++)
                {
                    object? expectedElement = i < expectedValues.Length ? expectedValues[i] : "<end of collection>";
                    object? actualElement = i < actualValues.Length ? actualValues[i] : "<end of collection>";

                    path.Push($"[{i}]");
                    CheckAreEqualCore(expectedElement, actualElement, path);
                    path.Pop();
                }
            }

            if (type.GetProperty("EqualityContract", BindingFlags.Instance | BindingFlags.NonPublic, null, returnType: typeof(Type), types: Array.Empty<Type>(), null) != null)
            {
                // Type is a C# record, run pointwise equality comparison.
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    path.Push("." + property.Name);
                    CheckAreEqualCore(property.GetValue(expected), property.GetValue(actual), path);
                    path.Pop();
                }

                return;
            }

            if (!expected.Equals(actual))
            {
                FailNotEqual();
            }

            void FailNotEqual() => Fail($"Value not equal in ${string.Join("", path.Reverse())}: expected {expected}, but was {actual}.");
        }
    }
}