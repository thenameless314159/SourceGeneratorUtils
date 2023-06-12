namespace SourceGeneratorUtils;

/// <summary>
/// Represents a strongly-typed <see cref="SourceBlockGenerator"/> for custom <see cref="ITypeSpec"/> descriptors.
/// </summary>
/// <typeparam name="TDescriptor">The type of <see cref="ITypeSpec"/> to generate a source block for.</typeparam>
public abstract class SourceBlockGenerator<TDescriptor> : SourceBlockGenerator where TDescriptor : ITypeSpec
{
    protected abstract void GenerateBlock(SourceWriter writer, in TypedSourceWritingContext context);

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