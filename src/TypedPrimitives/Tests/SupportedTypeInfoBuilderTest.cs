// Module Name: SupportedTypeInfoBuilderTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using FluentAssertions;

public class SupportedTypeInfoBuilderTest
{
  #region Tests

  [Fact]
  public void AddConverterCustomMacros_ShouldAccumulateMacros_ForSameConverter()
  {
    var info = new SupportedTypeInfoBuilder( typeof( int ) )
               .AddConverterCustomMacros( TypedPrimitiveConverter.TypeConverter, ( "M1", "V1" ) )
               .AddConverterCustomMacros( TypedPrimitiveConverter.TypeConverter, ( "M2", "V2" ) )
               .Build();

    info.GetConverterMacros( TypedPrimitiveConverter.TypeConverter )
        .Should()
        .BeEquivalentTo(
          new Dictionary<string, string>
          {
            { "M1", "V1" },
            { "M2", "V2" }
          }
        );
  }

  [Fact]
  public void AddConverterCustomMacros_ShouldThrow_WhenDuplicateMacroNamesProvidedForSameConverter()
  {
    var builder = new SupportedTypeInfoBuilder( typeof( int ) );

    var act = () => builder.AddConverterCustomMacros(
      TypedPrimitiveConverter.TypeConverter,
      ( "Dup", "1" ),
      ( "Dup", "2" )
    );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void AddIncludes_ShouldAccumulateIncludes_ForSameConverter()
  {
    var info = new SupportedTypeInfoBuilder( typeof( int ) )
               .AddIncludes( TypedPrimitiveConverter.TypeConverter, ( "I1", "V1" ) )
               .AddIncludes( TypedPrimitiveConverter.TypeConverter, ( "I2", "V2" ) )
               .Build();

    info.GetIncludes( TypedPrimitiveConverter.TypeConverter )
        .Should()
        .BeEquivalentTo(
          new Dictionary<string, string>
          {
            { "I1", "V1" },
            { "I2", "V2" }
          }
        );
  }

  [Fact]
  public void AddIncludes_ShouldThrow_WhenDuplicateIncludeNamesProvidedForSameConverter()
  {
    var builder = new SupportedTypeInfoBuilder( typeof( int ) );
    var act = () => builder.AddIncludes( TypedPrimitiveConverter.TypeConverter, ( "I", "1" ), ( "I", "2" ) );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void AddTypeKeyword_ShouldOverridePrimitiveMapping()
  {
    var info = new SupportedTypeInfoBuilder( typeof( int ) )
               .AddTypeKeyword( "myInt" )
               .Build();

    info.Keyword.Should().Be( "myInt" );
  }

  [Fact]
  public void AddTypeKeyword_ShouldUseLastValue_WhenCalledMultipleTimes()
  {
    var builder = new SupportedTypeInfoBuilder( typeof( int ) )
                  .AddTypeKeyword( "first" )
                  .AddTypeKeyword( "second" );

    var info = builder.Build();
    info.Keyword.Should().Be( "second" );
  }

  [Fact]
  public void Build_ShouldMaintainIsolation_BetweenDifferentConverters()
  {
    var info = new SupportedTypeInfoBuilder( typeof( int ) )
               .AddConverterCustomMacros( TypedPrimitiveConverter.TypeConverter, ( "M", "V" ) )
               .AddConverterCustomMacros( TypedPrimitiveConverter.SystemTextJson, ( "SM", "SV" ) )
               .AddIncludes( TypedPrimitiveConverter.TypeConverter, ( "I", "V" ) )
               .AddIncludes( TypedPrimitiveConverter.EfCoreValueConverter, ( "EI", "EV" ) )
               .Build();

    info.GetConverterMacros( TypedPrimitiveConverter.TypeConverter )
        .Should()
        .ContainSingle( kv => kv.Key == "M" && kv.Value == "V" );

    info.GetConverterMacros( TypedPrimitiveConverter.SystemTextJson )
        .Should()
        .ContainSingle( kv => kv.Key == "SM" && kv.Value == "SV" );
    info.GetConverterMacros( TypedPrimitiveConverter.NewtonsoftJson ).Should().BeEmpty();

    info.GetIncludes( TypedPrimitiveConverter.TypeConverter )
        .Should()
        .ContainSingle( kv => kv.Key == "I" && kv.Value == "V" );

    info.GetIncludes( TypedPrimitiveConverter.EfCoreValueConverter )
        .Should()
        .ContainSingle( kv => kv.Key == "EI" && kv.Value == "EV" );
    info.GetIncludes( TypedPrimitiveConverter.SystemTextJson ).Should().BeEmpty();
  }

  [Fact]
  public void Build_ShouldPrefixGlobalQualifier_WhenTypeNotPrimitive()
  {
    var info = new SupportedTypeInfoBuilder( typeof( CustomType ) )
      .Build();

    info.Keyword.Should().Be( $"global::{typeof( CustomType ).FullName}" );
  }

  [Fact]
  public void Build_ShouldProduceInfoWithEmptyCollections_WhenNoMacrosOrIncludesAdded()
  {
    var info = new SupportedTypeInfoBuilder( typeof( int ) ).Build();

    info.GetConverterMacros( TypedPrimitiveConverter.TypeConverter ).Should().BeEmpty();
    info.GetIncludes( TypedPrimitiveConverter.TypeConverter ).Should().BeEmpty();
  }

  [Theory]
  [MemberData( nameof( Build_ShouldUseKeywordMapping_WhenPrimitiveTypeData ) )]
  public void Build_ShouldUseKeywordMapping_WhenPrimitiveType(
    Type primitiveType,
    string expectedKeyword )
  {
    var info = new SupportedTypeInfoBuilder( primitiveType )
      .Build();

    info.Keyword.Should().Be( expectedKeyword );
  }

  [Fact]
  public void FluentMethods_ShouldReturnSameInstance_ForChaining()
  {
    var builder = new SupportedTypeInfoBuilder( typeof( int ) );
    builder.AddTypeKeyword( "x" ).Should().BeSameAs( builder );
    builder.AddConverterCustomMacros( TypedPrimitiveConverter.TypeConverter, ( "A", "1" ) ).Should().BeSameAs( builder );
    builder.AddIncludes( TypedPrimitiveConverter.TypeConverter, ( "I", "v" ) ).Should().BeSameAs( builder );
  }

  #endregion

  #region Implementation

  public static TheoryData<Type, string> Build_ShouldUseKeywordMapping_WhenPrimitiveTypeData { get; } = new ()
  {
    { typeof( bool ), "bool" },
    { typeof( byte ), "byte" },
    { typeof( char ), "char" },
    { typeof( decimal ), "decimal" },
    { typeof( double ), "double" },
    { typeof( short ), "short" },
    { typeof( int ), "int" },
    { typeof( long ), "long" },
    { typeof( sbyte ), "sbyte" },
    { typeof( float ), "float" },
    { typeof( string ), "string" },
    { typeof( ushort ), "ushort" },
    { typeof( uint ), "uint" },
    { typeof( ulong ), "ulong" }
  };

  private sealed class CustomType
  {
  }

  #endregion
}
