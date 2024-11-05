// Module Name: PrimitiveSourceGenerationTestBase.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using Xunit.Abstractions;

public abstract class PrimitiveSourceGenerationTestBase<T>( ITestOutputHelper output )
{
  #region Tests

  [Fact]
  public Task Compilation_ShouldSucceed_WithExplicitConverters()
  {
    var source = $"""
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( {typeof( T ).FullName} ), Converters = TypedPrimitiveConverter.TypeConverter | TypedPrimitiveConverter.SystemTextJson | TypedPrimitiveConverter.EfCoreValueConverter | TypedPrimitiveConverter.NewtonsoftJson )]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task Compilation_ShouldSucceed_WithExplicitNoneConverters()
  {
    var source = $"""
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( {typeof( T ).FullName} ), Converters = TypedPrimitiveConverter.None)]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task Compilation_ShouldSucceed_WithImplicitNoneConverters()
  {
    var source = $"""
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        [TypedPrimitive(typeof( {typeof( T ).FullName} ))]
        public readonly partial struct Test;
      """;

    return SourceGeneratorTestHelper.VerifyAsync<TypedPrimitiveSourceGenerator>( source, output );
  }

  #endregion
}
