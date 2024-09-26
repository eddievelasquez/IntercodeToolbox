// Module Name: StringPrimitiveSourceGenerationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using Xunit.Abstractions;

public class StringPrimitiveSourceGenerationTests( ITestOutputHelper output )
{
  #region Tests

  [Fact]
  public Task OrdinalCaseSensitiveComparisons()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;
        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        StringComparison = System.StringComparison.Ordinal)]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithDefaultConvertersAndNoValidation()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( string ), Converters = TypedPrimitiveConverter.Default)]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithExplicitConvertersAndNoValidation()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( string ), Converters = TypedPrimitiveConverter.NewtonsoftJson)]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithNoConvertersAndNoValidation()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( string ), Converters = TypedPrimitiveConverter.None)]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithoutExplicitConvertersAndNoValidation()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( string ))]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  #endregion
}
