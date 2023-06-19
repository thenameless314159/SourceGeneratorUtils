# **SourceGeneratorUtils** [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=thenameless314159_SourceGeneratorUtils&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=thenameless314159_SourceGeneratorUtils) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=thenameless314159_SourceGeneratorUtils&metric=coverage)](https://sonarcloud.io/summary/new_code?id=thenameless314159_SourceGeneratorUtils) [![Dotnet Version](https://img.shields.io/badge/dotnet-netstandard2.0-blue)](https://learn.microsoft.com/fr-fr/dotnet/standard/net-standard?tabs=net-standard-2-0)

<p align="center">An essential library equipped with utility functions and helpers to aid in writing source files for source generators or for general purposes.</p>

# Context and goals

In my journey of implementing [source generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview), I noticed recurrent similarities in each project. Handling source generation on nested types, for example, appeared frequently. Rewriting this logic on every source generator felt redundant and time-consuming, prompting me to abstract and manage most of these routine writing tasks. Hence, SourceGeneratorUtils was created. 

The main objective is to enable developers to dive straight into writing the code necessary for their source generators, instead of handling the boilerplate writing logic for namespaces, containing types etc.. With **SourceGeneratorUtils**, users can supply an implementation of [`AbstractGenerationSpec`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/AbstractGenerationSpec.cs) that could be mapped from Roslyn's [`ISymbol`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol?view=roslyn-dotnet-4.6.0) (for source generators) or some parsed files with type definitions (for general purpose). Users can then focus on implementing [`SourceCodeEmitter<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceCodeEmitter.cs)s with logic specific to the target code to generate.

# Installation

## NuGet Releases [![NuGet Badge](https://buildstats.info/nuget/SourceGeneratorUtils)](https://www.nuget.org/packages/SourceGeneratorUtils/)

This package can be found on [nuget.org](https://www.nuget.org/packages/SourceGeneratorUtils):

``` console
> dotnet add package SourceGeneratorUtils
```

## CI Builds [![CI status](https://github.com/thenameless314159/SourceGeneratorUtils/actions/workflows/ci.yml/badge.svg)](https://github.com/thenameless314159/SourceGeneratorUtils/actions/workflows/ci.yml)
For those who want to utilize builds from this repository's main branch, install them from the [**NuGet Github Package Registry**](https://docs.github.com/en/enterprise-server@3.8/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry) using [this repo's package feed](https://github.com/thenameless314159/SourceGeneratorUtils/pkgs/nuget/SourceGeneratorUtils). 
You'll need your own *Github Personal Access Token* (PAT) in order to access the registry. For information on obtaining your PAT, see the [**Working with the NuGet registry**](https://docs.github.com/en/enterprise-server@3.8/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#installing-a-package) article.

1. At the command line, navigate to your user profile directory and run the following command to add the package feed to your NuGet configuration, replacing the `<GITHUB_USER_NAME>` and `<PERSONAL_ACCESS_TOKEN>` placeholders with the relevant values:
    ``` shell
    > dotnet nuget add source -n GitHub -u <GITHUB_USER_NAME> -p <PERSONAL_ACCESS_TOKEN> https://nuget.pkg.github.com/thenameless314159/index.json
    ```
2. You should now be able to add a reference to the package specifying a version from the [repository packages feed](https://github.com/thenameless314159/SourceGeneratorUtils/pkgs/nuget/SourceGeneratorUtils)

# What's Included?
This library provides a variety of types that help the conception of source generation logic :

- [`SourceWriter`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceWriter.cs) - A minimal wrapper over StringBuilder, it handles indentation in a straightforward manner.
- [`SourceBuilder`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceBuilder.cs) - Another thin wrapper, this time over a dictionary, to store generated source files and export them to disk. The following type can populate it:
- [`SourceFileEmitterBase<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitterBase.cs) - Encapsulates all the necessary logic to write a C# source file ready for compilation. This is the main abstraction that users should implement to write their own source generation logic.
- [`SourceCodeEmitter<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceCodeEmitter.cs) - An abstraction that allows developers to break down their source generation logic into smaller components. It is used by [`SourceFileEmitter<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitter.cs) to write the source file.
- [`SourceFileEmitterOptions`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitterOptions.cs) - A simple record that holds options for source file generation within a `SourceFileEmitterBase<TSpec>`.
- [`TypeSourceFileEmitter`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/TypeSourceFileEmitter.cs) - A configurable abstraction that takes care of everything up until the target type body declaration.
- [`TypeSourceFileEmitterOptions`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/TypeSourceFileEmitterOptions.cs) - A simple record that holds options for source file generation within a `TypeSourceFileEmitter<TSpec>`.

# Getting started
Comprehensive documentation, along with illustrative examples (for the provided abstractions), is currently in progress and will be available soon.

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

# Acknowledgements
- I would like to thank [Damian Edwards](https://github.com/DamianEdwards) for the inspiration I drew from his CI workflows.
- Gratitude is also extended to the individual who wrote [`SourceWriter`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/gen/Helpers/SourceWriter.cs) at Microsoft and all the team behind the [`System.Text.Json`](https://github.com/dotnet/runtime/tree/main/src/libraries/System.Text.Json/gen) source generator.
