namespace SourceGeneratorUtils;

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