// Module Name: DiagnosticsSourceGenerationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using FluentAssertions;
using Xunit.Abstractions;

public class DiagnosticsSourceGenerationTests( ITestOutputHelper output )
{
  #region Tests

  [Fact]
  public async Task Compilation_ShouldFail_WhenPrimitiveTypeIsNotSupported()
  {
    var source = """
        namespace GeneratorTest;
      
        using Intercode.Toolbox.TypedPrimitives;
        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;
      
        [TypedPrimitive(typeof(string[]))]
        public readonly partial struct Test;
      """;

    var act = () => SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );

    await act.Should()
             .ThrowAsync<CompilationException>()
             .WithMessage( "The string[] type is not supported" );
  }

  [Fact]
  public async Task Compilation_ShouldFail_WhenPrimitiveTypeIsNotSupported2()
  {
    var source = """
        namespace GeneratorTest;
      
        using Intercode.Toolbox.TypedPrimitives;
        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;
      
        [TypedPrimitive(typeof(System.Net.IPAddress))]
        public readonly partial struct Test;
      """;

    var act = () => SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );

    await act.Should()
             .ThrowAsync<CompilationException>()
             .WithMessage( "The System.Net.IPAddress type is not supported" );
  }

  [Fact]
  public async Task Compilation_ShouldFail_WhenStructIsNotPartial()
  {
    var source = """
        namespace GeneratorTest;
      
        using Intercode.Toolbox.TypedPrimitives;
        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;
      
        [TypedPrimitive(typeof( int ))]
        public readonly struct Test;
      """;

    var act = () => SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );

    await act.Should()
             .ThrowAsync<CompilationException>()
             .WithMessage( "The 'GeneratorTest.Test' struct must be partial" );
  }

  [Fact]
  public async Task Compilation_ShouldFail_WhenStructIsNotReadOnly()
  {
    var source = """
        namespace GeneratorTest;
      
        using Intercode.Toolbox.TypedPrimitives;
        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;
      
        [TypedPrimitive(typeof( int ))]
        public partial struct Test;
      """;

    var act = () => SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );

    await act.Should()
             .ThrowAsync<CompilationException>()
             .WithMessage( "The 'GeneratorTest.Test' struct must be readonly" );
  }

  #endregion
}
