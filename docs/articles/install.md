# Installation
To properly set up a NuGet package **within a source generator assembly**, regardless of which registry it came from, some modifications are required in the target .csproj file. These modifications are necessary because dependencies cannot be added seamlessly to .NET Standard 2.0 source generators as described in [dotnet/roslyn#47517](https://github.com/dotnet/roslyn/discussions/47517). Follow these steps:

1. After adding the package via `dotnet add` or your IDE's NuGet Package Manager, set the `GeneratePathProperty` to true and embed all the `PrivateAssets` in the target `PackageReference` xml property :
   ``` xml
   <PackageReference Include="SourceGeneratorUtils" Version="1.3.0" GeneratePathProperty="true" PrivateAssets="all" />
   ```
2. Include two additional sections in the .csproj file:
   ``` xml
   <PropertyGroup>
       <GetTargetPathDependsOn>$(GetTargetPathDependsOn);GetDependencyTargetPaths</GetTargetPathDependsOn>
   </PropertyGroup>
   <Target Name="GetDependencyTargetPaths">
       <ItemGroup>
           <TargetPathWithTargetPlatformMoniker Include="$(PKGSourceGeneratorUtils)\lib\netstandard2.0\SourceGeneratorUtils.dll" IncludeRuntimeDependency="false" />
       </ItemGroup>
   </Target>
   ```

By following these steps, you can properly set up any NuGet package within a source generator assembly, regardless of its registry (given that you replace the assembly name and version with your own). These modifications ensure that the package's dependencies are handled correctly within the source generator.

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