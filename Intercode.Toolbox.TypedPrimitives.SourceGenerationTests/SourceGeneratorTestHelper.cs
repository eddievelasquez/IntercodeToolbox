// Module Name: SourceGeneratorTestHelper.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit.Abstractions;

internal static class SourceGeneratorTestHelper
{
  #region Public Methods

  public static Task Verify<TGenerator>(
    string source,
    ITestOutputHelper? output = null )
    where TGenerator: IIncrementalGenerator, new()
  {
    var syntaxTree = CSharpSyntaxTree.ParseText( source );
    var assemblies = AppDomain.CurrentDomain.GetAssemblies();

    var references =
      assemblies.Where( assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace( assembly.Location ) )
                .Select( assembly => MetadataReference.CreateFromFile( assembly.Location ) )
                .Cast<MetadataReference>()
                .ToList();

    var compilation = CSharpCompilation.Create(
      "SourceGeneratorTests",
      [syntaxTree],
      references,
      new CSharpCompilationOptions( OutputKind.DynamicallyLinkedLibrary )
    );

    // Ensure the incoming code has no errors
    var compileDiagnostics = compilation.GetDiagnostics();
    var diagnostics = compileDiagnostics.Where( d => d.Severity == DiagnosticSeverity.Error )
                                        .ToList();

    diagnostics.Should()
               .BeEmpty();

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    var generator = new TGenerator();

    GeneratorDriver driver = CSharpGeneratorDriver.Create( generator );
    driver = driver.RunGenerators( compilation );

    stopwatch.Stop();
    output?.WriteLine( $"Source generation took: {stopwatch.Elapsed.TotalMilliseconds:f2}ms" );

    // Ensure the generated code has no errors
    diagnostics = driver.GetRunResult()
                        .Diagnostics.Where( d => d.Severity == DiagnosticSeverity.Error )
                        .ToList();

    diagnostics.Should()
               .BeEmpty();

    // Verify the generated code vs the expected code
    return Verifier.Verify( driver )
                   //.AutoVerify()
                   .UseDirectory( GetSnapshotDirectory( Assembly.GetCallingAssembly() ) );
  }

  #endregion

  #region Implementation

  private static string GetSnapshotDirectory(
    Assembly assembly )
  {
    var path = assembly.Location;
    var pos = path.IndexOf( "bin", StringComparison.OrdinalIgnoreCase );
    if( pos == -1 )
    {
      throw new InvalidOperationException( "Could not find the 'bin' directory in the assembly location" );
    }

    var snapshotDirectory = Path.Combine( path[..pos], "Snapshots" );
    return snapshotDirectory;
  }

  #endregion
}
