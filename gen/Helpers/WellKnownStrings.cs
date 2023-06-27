﻿namespace SourceGeneratorUtils.SourceGeneration;

internal static class WellKnownStrings
{
    public const string DescriptorsNamespace = "SourceGeneratorUtils.SourceGeneration.Infrastructure.Descriptors";
    public const string ExtensionsNamespace = "SourceGeneratorUtils.SourceGeneration.Infrastructure.Extensions";
    public const string InfrastructureNamespace = "SourceGeneratorUtils.SourceGeneration.Infrastructure";
    public const string LocalAssemblyNamespace = "SourceGeneratorUtils.SourceGeneration";

    public const string SourceFileHeader = """
        // <auto-generated>
        //     Generated by https://github.com/thenameless314159/SourceGeneratorUtils
        //     Any changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
        // </auto-generated>
        """;
}