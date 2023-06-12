namespace SourceGeneratorUtils;

/// <summary>
/// Represents a strongly-typed <see cref="SourceBlockGenerator"/> for custom <see cref="ITypeSpec"/> descriptors.
/// </summary>
/// <typeparam name="TDescriptor">The type of <see cref="ITypeSpec"/> to generate a source block for.</typeparam>
public abstract class SourceBlockGenerator<TDescriptor> : SourceBlockGenerator where TDescriptor : ITypeSpec
{
    /// <summary>
    /// Generates the source block for the target <see cref="ITypeSpec"/> using the strongly-typed <see cref="TypedSourceWritingContext"/>.
    /// </summary>
    /// <param name="writer">The target source writer.</param>
    /// <param name="context">The source writing context.</param>
    protected abstract void GenerateBlock(SourceWriter writer, in TypedSourceWritingContext context);

    /// <summary>
    /// Represents a strongly-typed <see cref="SourceWritingContext"/> for custom <see cref="ITypeSpec"/> descriptors.
    /// </summary>
    /// <param name="Descriptor">The target <see cref="ITypeSpec"/> to generate source for.</param>
    /// <param name="Options">The <see cref="SourceFileGenOptions"/> used in this context.</param>
    /// <param name="Descriptors">The types that are being generated in this context.</param>
    protected readonly record struct TypedSourceWritingContext(
        TDescriptor Descriptor, 
        SourceFileGenOptions Options, 
        IReadOnlyDictionary<string, TDescriptor> Descriptors);

    /// <inheritdoc />
    public override void GenerateBlock(SourceWriter writer, in SourceWritingContext context)
    {
        if (context.Target is not TDescriptor typedTarget)
            throw new ArgumentOutOfRangeException(nameof(context), 
                $"Invalid target type from {nameof(SourceWritingContext)}. " +
                $"Expected target to be of type {typeof(TDescriptor).Name} but was {context.Target.GetType().Name}");

        if (context.Types is not DescriptorStore<TDescriptor> typedDescriptors)
            throw new ArgumentOutOfRangeException(nameof(context),
                $"Invalid types store from {nameof(SourceWritingContext)}.");

        TypedSourceWritingContext typedContext = new(typedTarget, context.Options, typedDescriptors._descriptors);
        GenerateBlock(writer, in typedContext);
    }
}