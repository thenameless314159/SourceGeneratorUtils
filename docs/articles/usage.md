# Getting started
Comprehensive documentation, along with illustrative examples (for the provided abstractions), is currently in progress and will be available soon.

## What's Included?
This library provides a variety of types that help the conception of source generation logic :

- [`SourceWriter`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceWriter.cs) - A minimal wrapper over StringBuilder, it handles indentation in a straightforward manner.
- [`SourceBuilder`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceBuilder.cs) - Another thin wrapper, this time over a dictionary, to store generated source files and export them to disk. The following type can populate it:
- [`SourceFileEmitterBase<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitterBase.cs) - Encapsulates all the necessary logic to write a C# source file ready for compilation. This is the main abstraction that users should implement to write their own source generation logic.
- [`SourceCodeEmitter<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceCodeEmitter.cs) - An abstraction that allows developers to break down their source generation logic into smaller components. It is used by [`SourceFileEmitter<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitter.cs) to write the source file.
- [`SourceFileEmitterOptions`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitterOptions.cs) - A simple record that holds options for source file generation within a `SourceFileEmitterBase<TSpec>`.
- [`TypeSourceFileEmitter`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/TypeSourceFileEmitter.cs) - A configurable abstraction that takes care of everything up until the target type body declaration.
- [`TypeSourceFileEmitterOptions`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/TypeSourceFileEmitterOptions.cs) - A simple record that holds options for source file generation within a `TypeSourceFileEmitter<TSpec>`.

## Default implementation usage
**SourceGeneratorUtils** offers a default implementation for the provided abstraction, in the form of [`DefaultSourceFileEmitter`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/DefaultSourceFileEmitter.cs). This implementation has been meticulously crafted to convert [`DefaultGenerationSpec`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/DefaultGenerationSpec.cs) instances into production-ready, compile-ready C# source code.
The DefaultGenerationSpec record can be manually constructed, which requires the user to provide `TypeDeclarations` strings (to handle type and containing types declarations) manually along with an [`ITypeDescriptor`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/Infrastructure/Descriptors/ITypeDescriptor.cs) reference for the target type. Alternatively, it can also be built from a target [`TypeDesc`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/Infrastructure/Descriptors/TypeDesc.cs) using the `DefaultGenerationSpec.CreateFrom(TypeDesc,params ITypeDescriptor[])` static factory method.

To illustrate, let's delve into the process of **generating a source file** for a simple record with a single property. Suppose we need to generate sources for the following class:

``` csharp
namespace MyNamespace;

internal partial class MyClass
{
    [MyGeneratorAttribute]
    protected partial record MyRecord
    {
        public required string MyProperty { get; init; }
    }
}
```

First, we need to implement a [`DefaultSourceCodeEmitter`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/DefaultSourceCodeEmitter.cs) with the logic that we want to incorporate into our target type. Here's an example:

``` csharp
sealed class MyInterfaceImplementation : DefaultSourceCodeEmitter
{
    public override IEnumerable<string> GetInterfacesToImplement(DefaultGenerationSpec target)
    {
        yield return "IMyInterface";
    }

    public override void EmitTargetSourceCode(DefaultGenerationSpec target, SourceWriter writer)
    {
        writer.WriteLine("public void MyMethod()");
        writer.OpenBlock();
        writer.WriteLine("throw new global::System.NotImplementedException();");
        writer.CloseBlock(); // last block may not need to be closed as they'll be closed by the emitter
    }
}
```

Next, within the source generator context, we map the target type as a [`TypeDesc`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/Infrastructure/Descriptors/TypeDesc.cs) and create a DefaultGenerationSpec from it, as shown below:

``` csharp
TypeDesc myClassDesc = new() // some properties are omitted for brevity
{
    IsRecord = true,
    Name = "MyRecord",
    Namespace = "MyNamespace",
    TypeModifier = "partial",
    TypeKind = TypeKind.Class,
    Accessibility = Accessibility.Protected,
    Attributes = ImmutableEquatableArray.Create(new TypeDesc { Name = "MyGeneratorAttribute", }),

    ContainingTypes = ImmutableEquatableArray.Create(new TypeDesc
    {
        Name = "MyClass",
        Namespace = "MyNamespace",
        TypeModifier = "partial", // may become IsPartial and IsSealed bool properties in the future
        TypeKind = TypeKind.Class,
        Accessibility = Accessibility.Internal,
    })
};
```

Finally, we can tie everything together using the bundled [`DefaultSourceFileEmitter`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/DefaultSourceFileEmitter.cs) and [`SourceBuilder`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceBuilder.cs) to emit the source file:

``` csharp
var options = new TypeSourceFileEmitterOptions { AssemblyName = typeof(MyGenerator).Assembly.GetName(), UseFileScopedNamespace = true };
var sourceEmitter = new DefaultSourceFileEmitter(options) { SourceCodeEmitters = new[] { new MyInterfaceImplementation() } };
var sourceBuilder = new SourceBuilder().Register(sourceEmitter, DefaultGenerationSpec.CreateFrom(myClassDesc));
sourceBuilder.ExportTo(Directory.GetCurrentDirectory());
```

This will generate a file named `MyRecord.g.cs` in the current directory with the following content:

``` csharp
// <auto-generated/>

#nullable enable annotations
#nullable disable warnings

namespace MyNamespace;

internal partial class MyClass
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MyGenerator", "1.0.0.0")]
    protected partial record MyRecord : IMyInterface
    {
        public void MyMethod()
        {
            throw new global::System.NotImplementedException();
        }
    }
}
```

In this way, you can leverage my library to efficiently generate C# source files from specifications, saving time and ensuring consistency in your code. 
Remember, this is just an example and may not perfectly fit your project. The important point is to provide a useful overview and some guidance on how to use the default implementation.