// Module Name: StringPrimitiveSourceGenerationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using Xunit.Abstractions;

public class StringPrimitiveSourceGenerationTests( ITestOutputHelper output )
{
  #region Tests

  [Fact]
  public Task Compilation_ShouldSucceed_WithExplicitDefaultConverters()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( string ), Converters = TypedPrimitiveConverter.Default)]
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

        [TypedPrimitive(typeof( string ), Converters = TypedPrimitiveConverter.None)]
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

        [TypedPrimitive(typeof( string ))]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task Compilation_ShouldSucceed_WithNewtonsoftJsonConverter()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( string ), Converters = TypedPrimitiveConverter.NewtonsoftJson)]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task Compilation_ShouldSucceed_WithStringComparison()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;
        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        StringComparison = System.StringComparison.Ordinal)]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  #endregion
}
