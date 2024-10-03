// Module Name: DateTimePrimitiveSourceGenerationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using Xunit.Abstractions;

public class DateTimePrimitiveSourceGenerationTests( ITestOutputHelper output )
{
  #region Tests

  [Fact]
  public Task Compilation_ShouldSucceed_WithExplicitConverters()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( System.DateTime ), Converters = TypedPrimitiveConverter.TypeConverter | TypedPrimitiveConverter.SystemTextJson | TypedPrimitiveConverter.EfCoreValueConverter)]
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

        [TypedPrimitive(typeof( System.DateTime ), Converters = TypedPrimitiveConverter.None)]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task Compilation_ShouldSucceed_WithImplicitNoneConverters()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( System.DateTime ))]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  #endregion
}
