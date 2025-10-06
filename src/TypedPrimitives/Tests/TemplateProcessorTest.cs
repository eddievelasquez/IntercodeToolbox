// Module Name: TemplateProcessorTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using FluentAssertions;

public class TemplateProcessorTest
{
  #region Tests

  [Fact]
  public void GenerateTypes_ShouldIncludeOnlyEnabledAttributes()
  {
    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.TypeConverter | TypedPrimitiveConverter.SystemTextJson,
      null
    );

    var processor = new TemplateProcessor();
    var main = processor.GenerateTypes( model ).First().SourceText.ToString();

    main.Should().Contain( "TypeConverter(" );
    main.Should().Contain( "System.Text.Json.Serialization.JsonConverter(" );
    main.Should().NotContain( "Newtonsoft.Json.JsonConverter(" );
  }

  [Fact]
  public void GenerateTypes_ShouldNameConverterOutputsWithSuffix()
  {
    var processor = new TemplateProcessor();

    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.TypeConverter | TypedPrimitiveConverter.NewtonsoftJson,
      null
    );

    var generated = processor.GenerateTypes( model ).ToArray();

    generated.Select( g => g.TypeName ).Should().Contain( "Test.Namespace.MyInt" );
    generated.Select( g => g.TypeName ).Should().Contain( "Test.Namespace.MyInt" + ResourceNames.TypeConverterTemplate );

    generated.Select( g => g.TypeName )
             .Should()
             .Contain( "Test.Namespace.MyInt" + ResourceNames.NewtonsoftJsonConverterTemplate );
  }

  [Fact]
  public void GenerateTypes_ShouldProduceDifferentSource_ForDifferentPrimitiveTypes()
  {
    var processor = new TemplateProcessor();

    var intModel = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.TypeConverter,
      null
    );

    var stringModel = new GeneratorModel(
      typeof( string ),
      "MyString",
      "Test.Namespace",
      TypedPrimitiveConverter.TypeConverter,
      "StringComparison.Ordinal"
    );

    var intMain = processor.GenerateTypes( intModel ).First().SourceText.ToString();
    var stringMain = processor.GenerateTypes( stringModel ).First().SourceText.ToString();

    intMain.Should().NotBe( stringMain );
    intMain.Should().Contain( "IComparable<int>" );
    stringMain.Should().Contain( "IComparable<string>" );
  }

  [Fact]
  public void GenerateTypes_ShouldProduceDistinctMainSource_WhenConverterSetsDiffer()
  {
    var processor = new TemplateProcessor();

    var modelNone = new GeneratorModel( typeof( int ), "MyInt", "Test.Namespace", TypedPrimitiveConverter.None, null );

    var modelTypeConv = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.TypeConverter,
      null
    );
    var modelAll = new GeneratorModel( typeof( int ), "MyInt", "Test.Namespace", TypedPrimitiveConverter.All, null );

    var noneSrc = processor.GenerateTypes( modelNone ).First().SourceText.ToString();
    var typeConvSrc = processor.GenerateTypes( modelTypeConv ).First().SourceText.ToString();
    var allSrc = processor.GenerateTypes( modelAll ).First().SourceText.ToString();

    noneSrc.Should().NotBe( typeConvSrc );
    allSrc.Should().NotBe( typeConvSrc );
    allSrc.Should().NotBe( noneSrc );
  }

  [Theory]
  [MemberData( nameof( ConverterFlagData ) )]
  public void GenerateTypes_ShouldReturnExpectedCount_ForConverterFlags(
    TypedPrimitiveConverter flags,
    int expectedCount )
  {
    var model = new GeneratorModel( typeof( int ), "MyInt", "Test.Namespace", flags, null );
    var processor = new TemplateProcessor();

    var generated = processor.GenerateTypes( model ).ToArray();

    generated.Should().HaveCount( expectedCount );
    generated[0].TypeName.Should().Be( "Test.Namespace.MyInt" );
  }

  #endregion

  #region Implementation

  public static TheoryData<TypedPrimitiveConverter, int> ConverterFlagData =>
    new ()
    {
      { TypedPrimitiveConverter.None, 1 }, // main only
      { TypedPrimitiveConverter.TypeConverter, 2 }, // main + 1
      { TypedPrimitiveConverter.SystemTextJson, 2 }, // main + 1
      { TypedPrimitiveConverter.NewtonsoftJson, 2 }, // main + 1
      { TypedPrimitiveConverter.EfCoreValueConverter, 2 }, // main + 1
      { TypedPrimitiveConverter.TypeConverter | TypedPrimitiveConverter.SystemTextJson, 3 },
      { TypedPrimitiveConverter.TypeConverter | TypedPrimitiveConverter.NewtonsoftJson, 3 },
      { TypedPrimitiveConverter.SystemTextJson | TypedPrimitiveConverter.NewtonsoftJson, 3 },
      { TypedPrimitiveConverter.All, 5 } // main + 4
    };

  #endregion
}
