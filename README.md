# **SourceGeneratorUtils** [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=thenameless314159_SourceGeneratorUtils&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=thenameless314159_SourceGeneratorUtils) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=thenameless314159_SourceGeneratorUtils&metric=coverage)](https://sonarcloud.io/summary/new_code?id=thenameless314159_SourceGeneratorUtils) [![Dotnet Version](https://img.shields.io/badge/dotnet-netstandard2.0-blue)](https://learn.microsoft.com/fr-fr/dotnet/standard/net-standard?tabs=net-standard-2-0)

<p align="center">An essential library equipped with utility functions and helpers to aid in writing source files for source generators or for general purposes.</p>

# Context and goals

In my journey of implementing [source generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview), I noticed recurrent similarities in each project. Handling source generation on nested types, for example, appeared frequently. Rewriting this logic on every source generator felt redundant and time-consuming, prompting me to abstract and manage most of these routine writing tasks. Hence, SourceGeneratorUtils was created. 

The main objective is to enable developers to dive straight into writing the code necessary for their source generators, instead of handling the boilerplate writing logic for namespaces, containing types etc.. With **SourceGeneratorUtils**, developers can supply a populated implementation of [`AbstractGenerationSpec`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/AbstractGenerationSpec.cs) (or [`AbstractTypeGenerationSpec`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/AbstractTypeGenerationSpec.cs)) that could be mapped from Roslyn's [`ISymbol`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol?view=roslyn-dotnet-4.6.0) (for source generators) or some parsed files with type definitions (for general purpose). They can then focus on implementing [`SourceCodeEmitter<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceCodeEmitter.cs)s components (used by [`SourceFileEmitter<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitter.cs)) with logic specific to the target code to generate.

# Installation
In order to **import** this library in your [source generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview), some small tweaks are required. Full details can be found on [this repository's detailed documentation](https://thenameless314159.github.io/SourceGeneratorUtils/articles/install.html).

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

# Getting started

**An article** explaining the **default implementation usage** is available on [this repository's detailed documentation](https://thenameless314159.github.io/SourceGeneratorUtils/articles/usage.html).

Comprehensive documentation, along with illustrative examples (for the provided abstractions), is currently in progress and will be available soon.

## What's Included?
This library provides a variety of types that help the conception of source generation logic :

- [`SourceWriter`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceWriter.cs) - A minimal wrapper over StringBuilder, it handles indentation in a straightforward manner.
- [`SourceBuilder`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceBuilder.cs) - Another thin wrapper, this time over a dictionary, to store generated source files and export them to disk. The following type can populate it:
- [`SourceFileEmitterBase<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitterBase.cs) - Base abstraction to encapsulate all the necessary logic to write a C# source file ready for compilation. This abstraction should be used if you don't need target types declaration as well as `SourceCodeEmitter<TSpec>` components.
- [`SourceFileEmitterBaseOptions`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitterBaseOptions.cs) - A simple record that holds options for source file generation within a `SourceFileEmitterBase<TSpec>`.
- [`SourceCodeEmitter<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceCodeEmitter.cs) - An abstraction that allows developers to break down their source generation logic into smaller reusable components. This type is used by the following:
- [`SourceFileEmitter<TSpec>`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitter.cs) An abstraction encapsulating all the logic necessary to generate ready-to-compile source files for the given target.
- [`SourceFileEmitterOptions`](https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/SourceFileEmitterOptions.cs) - A simple record that holds options for source file generation within a `SourceFileEmitter<TSpec>`.

# Acknowledgements
- I would like to thank [Damian Edwards](https://github.com/DamianEdwards) for the inspiration I drew from his CI workflows.
- Gratitude is also extended to the individual who wrote [`SourceWriter`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/gen/Helpers/SourceWriter.cs) at Microsoft and all the team behind the [`System.Text.Json`](https://github.com/dotnet/runtime/tree/main/src/libraries/System.Text.Json/gen) source generator.
