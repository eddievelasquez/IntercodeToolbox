// Module Name: LongPrimitiveSourceGenerationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using Xunit.Abstractions;

public class LongPrimitiveSourceGenerationTests( ITestOutputHelper output )
{
  #region Tests

  [Fact]
  public Task Compilation_ShouldSucceed_WithExplicitDefaultConverters()
  {
    var source = """
        namespace GeneratorTest;
      
        using Intercode.Toolbox.TypedPrimitives;
      
        [TypedPrimitive(typeof( long ), Converters = TypedPrimitiveConverter.Default)]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task Compilation_ShouldSucceed_WithExplicitNoneConverters()
  {
    var source = """
        namespace GeneratorTest;
      
        using Intercode.Toolbox.TypedPrimitives;
      
        [TypedPrimitive(typeof( long ), Converters = TypedPrimitiveConverter.None)]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task Compilation_ShouldSucceed_WithImplicitDefaultConverters()
  {
    var source = """
        namespace GeneratorTest;
      
        using Intercode.Toolbox.TypedPrimitives;
      
        [TypedPrimitive(typeof( long ))]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  #endregion
}
