// Module Name: GeneratorModelTest.cs
// Author:      GitHub Copilot
//
// Tests for Intercode.Toolbox.TypedPrimitives.GeneratorModel

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using FluentAssertions;
using Intercode.Toolbox.TypedPrimitives;
using System.Linq;

public class GeneratorModelTest
{
  [Fact]
  public void Ctor_ShouldSetBasicProperties_ForValueType()
  {
    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.None,
      "StringComparison.Ordinal" );

    model.PrimitiveType.Should().Be( typeof( int ) );
    model.PrimitiveTypeName.Should().Be( typeof( int ).FullName );
    model.IsValueType.Should().BeTrue();
    model.TypeName.Should().Be( "MyInt" );
    model.Namespace.Should().Be( "Test.Namespace" );
    model.Converters.Should().Be( TypedPrimitiveConverter.None );
    model.StringComparison.Should().Be( "StringComparison.Ordinal" );
  }

  [Fact]
  public void Ctor_ShouldSetBasicProperties_ForReferenceType()
  {
    var model = new GeneratorModel(
      typeof( string ),
      "MyString",
      "Test.Namespace",
      TypedPrimitiveConverter.None,
      null );

    model.PrimitiveType.Should().Be( typeof( string ) );
    model.PrimitiveTypeName.Should().Be( typeof( string ).FullName );
    model.IsValueType.Should().BeFalse();
    model.TypeName.Should().Be( "MyString" );
    model.Namespace.Should().Be( "Test.Namespace" );
    model.Converters.Should().Be( TypedPrimitiveConverter.None );
    model.StringComparison.Should().BeNull();
  }

  [Fact]
  public void Ctor_ShouldEnableSpecifiedConverters()
  {
    var enabled = TypedPrimitiveConverter.TypeConverter | TypedPrimitiveConverter.NewtonsoftJson;

    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      enabled,
      null );

    model.TypeConverter.IsEnabled.Should().BeTrue();
    model.NewtonsoftJsonConverter.IsEnabled.Should().BeTrue();
    model.SystemTextJsonConverter.IsEnabled.Should().BeFalse();
    model.EfCoreValueConverter.IsEnabled.Should().BeFalse();
  }

  [Fact]
  public void Ctor_ShouldEnableAllConverters_WhenAllFlagUsed()
  {
    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.All,
      null );

    model.TypeConverter.IsEnabled.Should().BeTrue();
    model.SystemTextJsonConverter.IsEnabled.Should().BeTrue();
    model.NewtonsoftJsonConverter.IsEnabled.Should().BeTrue();
    model.EfCoreValueConverter.IsEnabled.Should().BeTrue();
  }

  [Fact]
  public void Ctor_ShouldDisableAllConverters_WhenNoneFlagUsed()
  {
    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.None,
      null );

    model.TypeConverter.IsEnabled.Should().BeFalse();
    model.SystemTextJsonConverter.IsEnabled.Should().BeFalse();
    model.NewtonsoftJsonConverter.IsEnabled.Should().BeFalse();
    model.EfCoreValueConverter.IsEnabled.Should().BeFalse();
  }

  [Theory]
  [InlineData( TypedPrimitiveConverter.TypeConverter )]
  [InlineData( TypedPrimitiveConverter.SystemTextJson )]
  [InlineData( TypedPrimitiveConverter.NewtonsoftJson )]
  [InlineData( TypedPrimitiveConverter.EfCoreValueConverter )]
  public void GetEnabledConverters_ShouldReturnSingleConverter_WhenOnlyOneFlagSet(
    TypedPrimitiveConverter flag )
  {
    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      flag,
      null );

    var converters = model.GetEnabledConverters().ToArray();

    converters.Should().ContainSingle();

    var expected = flag switch
    {
      TypedPrimitiveConverter.TypeConverter => TemplateType.TypeConverter,
      TypedPrimitiveConverter.SystemTextJson => TemplateType.SystemTextJsonConverter,
      TypedPrimitiveConverter.NewtonsoftJson => TemplateType.NewtonsoftJsonConverter,
      TypedPrimitiveConverter.EfCoreValueConverter => TemplateType.EfCoreValueConverter,
      _ => throw new InvalidOperationException()
    };

    converters[0].TemplateType.Should().Be( expected );
  }

  [Fact]
  public void GetEnabledConverters_ShouldReturnEnabledConverters_InDeclaredOrder()
  {
    var enabled = TypedPrimitiveConverter.NewtonsoftJson | TypedPrimitiveConverter.TypeConverter | TypedPrimitiveConverter.EfCoreValueConverter;

    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      enabled,
      null );

    var converters = model.GetEnabledConverters().ToArray();

    converters.Should().HaveCount( 3 );
    converters[0].TemplateType.Should().Be( TemplateType.TypeConverter );
    converters[1].TemplateType.Should().Be( TemplateType.NewtonsoftJsonConverter );
    converters[2].TemplateType.Should().Be( TemplateType.EfCoreValueConverter );
  }

  [Fact]
  public void GetEnabledConverters_ShouldIncludeOnlyMiddleConverters_WhenTypeConverterDisabled()
  {
    var enabled = TypedPrimitiveConverter.SystemTextJson | TypedPrimitiveConverter.NewtonsoftJson;

    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      enabled,
      null );

    var converters = model.GetEnabledConverters().ToArray();

    converters.Should().HaveCount( 2 );
    converters[0].TemplateType.Should().Be( TemplateType.SystemTextJsonConverter );
    converters[1].TemplateType.Should().Be( TemplateType.NewtonsoftJsonConverter );
  }

  [Fact]
  public void GetEnabledConverters_ShouldReturnConverters_InOrder_ForNonContiguousFlags()
  {
    var enabled = TypedPrimitiveConverter.SystemTextJson | TypedPrimitiveConverter.EfCoreValueConverter;

    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      enabled,
      null );

    var converters = model.GetEnabledConverters().ToArray();

    converters.Should().HaveCount( 2 );
    converters[0].TemplateType.Should().Be( TemplateType.SystemTextJsonConverter );
    converters[1].TemplateType.Should().Be( TemplateType.EfCoreValueConverter );
  }

  [Fact]
  public void GetEnabledConverters_ShouldReturnAllConverters_InFixedOrder_WhenAllEnabled()
  {
    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.All,
      null );

    var converters = model.GetEnabledConverters().ToArray();

    converters.Should().HaveCount( 4 );
    converters.Select( c => c.TemplateType ).Should().ContainInOrder(
      TemplateType.TypeConverter,
      TemplateType.SystemTextJsonConverter,
      TemplateType.NewtonsoftJsonConverter,
      TemplateType.EfCoreValueConverter );
  }

  [Fact]
  public void AllFlag_ShouldEqualBitwiseOrOfIndividualFlags()
  {
#if !NETSTANDARD2_0 // In annotations project All may not exist; guard if removed
    var combined = TypedPrimitiveConverter.TypeConverter | TypedPrimitiveConverter.SystemTextJson | TypedPrimitiveConverter.EfCoreValueConverter | TypedPrimitiveConverter.NewtonsoftJson;
    ( (int) TypedPrimitiveConverter.All ).Should().Be( (int) combined );
#endif
  }

  [Fact]
  public void GetEnabledConverters_ShouldReturnEmpty_WhenNoneEnabled()
  {
    var model = new GeneratorModel(
      typeof( int ),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.None,
      null );

    model.GetEnabledConverters().Should().BeEmpty();
  }
}
