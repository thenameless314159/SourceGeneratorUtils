﻿namespace SourceGeneratorUtils;

/// <summary>
/// Provides extension methods for <see cref="SourceBuilder"/> to populate it using <see cref="ISourceFileGenerator{TDescriptor}"/>.
/// </summary>
public static class SourceBuilderExtensions
{
    /// <summary>
    /// Populate the <paramref name="builder"/> with the source file generated by <paramref name="generator"/> for the given <paramref name="descriptor"/>.
    /// </summary>
    /// <typeparam name="TDescriptor">The type of the target descriptor to generate a source file for.</typeparam>
    /// <param name="builder">The <see cref="SourceBuilder"/>.</param>
    /// <param name="generator">The source file generator.</param>
    /// <param name="descriptor">The target descriptor to generate source for.</param>
    /// <returns>The current <see cref="SourceBuilder"/> instance.</returns>
    public static SourceBuilder Register<TDescriptor>(this SourceBuilder builder, ISourceFileGenerator<TDescriptor> generator,
        TDescriptor descriptor)
    {
        var sourceFile = generator.GenerateSource(descriptor);
        return builder.Register(in sourceFile);
    }

    /// <summary>
    /// Populate the <paramref name="builder"/> with the source file generated by <paramref name="generator"/> for the given <paramref name="descriptors"/>.
    /// </summary>
    /// <typeparam name="TDescriptor">The type of the target descriptor to generate a source file for.</typeparam>
    /// <param name="builder">The <see cref="SourceBuilder"/>.</param>
    /// <param name="generator">The source file generator.</param>
    /// <param name="descriptors">The target descriptors to generate source for.</param>
    /// <returns>The current <see cref="SourceBuilder"/> instance.</returns>
    public static SourceBuilder PopulateWith<TDescriptor>(this SourceBuilder builder, ISourceFileGenerator<TDescriptor> generator, 
        IEnumerable<TDescriptor> descriptors)
    {
        // fast path if the descriptors are contained within an array
        if (descriptors is TDescriptor[] descriptorArray)
        {
            foreach (ref readonly TDescriptor desc in descriptorArray.AsSpan())
                builder.Register(generator, desc);
        }
#if DOTNET7_0_OR_GREATER
        // fast path if the descriptors are contained within a list
        else if (sourceFiles is List<TDescriptor> descriptorList)
        {
            foreach (ref readonly TDescriptor desc in CollectionsMarshal.AsSpan(descriptorList))
                builder.Register(generator, in desc, relatives);
        }   
#endif
        else
        {
            foreach (TDescriptor descriptor in descriptors)
                builder.Register(generator, descriptor);
        }

        return builder;
    }
}