// Module Name: TypeConverterTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using System.ComponentModel;
using FluentAssertions;

public abstract class TypeConverterTests<TDataFactory>
  where TDataFactory: ITestDataFactory
{
  #region Nested Types

  private class UnsupportedType
  {
    #region Constructors

    private UnsupportedType(
      string value )
    {
      Value = value;
    }

    #endregion

    #region Properties

    public static UnsupportedType Instance => new ( "12345" );

    public string Value { get; set; }

    #endregion
  }

  #endregion

  #region Public Methods

  public static IEnumerable<object?[]> GetData(
    int take )
  {
    return TDataFactory.GetValidValues().Take( take );
  }

  #endregion

#pragma warning disable xUnit1026 // Don't warn about unused parameters

  [Theory]
  [MemberData( nameof( GetData ), 2 )]
  public void TypeConverter_CanConvertFrom_ReturnsTrue(
    object primitive,
    object value,
    bool expected )
  {
    var converter = TypeDescriptor.GetConverter( primitive.GetType() );

    converter.CanConvertFrom( value.GetType() )
             .Should()
             .Be( expected );
  }

  [Theory]
  [MemberData( nameof( GetData ), 2 )]
  public void TypeConverter_CanConvertTo_ReturnsTrue(
    object primitive,
    object value,
    bool expected )
  {
    var converter = TypeDescriptor.GetConverter( primitive.GetType() );

    converter.CanConvertTo( value.GetType() )
             .Should()
             .Be( expected );
  }

  [Theory]
  [MemberData( nameof( GetData ), 3 )]
  public void TypeConverter_ConvertFrom_Succeeds(
    object primitive,
    object value,
    bool expected )
  {
    var converter = TypeDescriptor.GetConverter( primitive.GetType() );
    var result = converter.ConvertFrom( value );

    result.Should().Be( primitive );
  }

  [Theory]
  [MemberData( nameof( GetData ), 1 )]
  public void TypeConverter_ConvertFrom_UnsupportedType_Throws(
    object primitive,
    object? value,
    bool expected )
  {
    var converter = TypeDescriptor.GetConverter( primitive.GetType() );

    var act = () => converter.ConvertFrom( UnsupportedType.Instance );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Theory]
  [MemberData( nameof( GetData ), 2 )]
  public void TypeConverter_ConvertTo_Succeeds(
    object primitive,
    object value,
    bool expected )
  {
    var converter = TypeDescriptor.GetConverter( primitive.GetType() );
    var result = converter.ConvertTo( null, null, primitive, value.GetType() );

    result.Should().Be( value );
  }

  [Theory]
  [MemberData( nameof( GetData ), 1 )]
  public void TypeConverter_ConvertTo_UnsupportedType_Throws(
    object primitive,
    object value,
    bool expected )
  {
    var converter = TypeDescriptor.GetConverter( primitive.GetType() );

    var act = () => converter.ConvertTo( null, null, primitive, typeof( UnsupportedType ) );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Theory]
  [MemberData( nameof( GetData ), 1 )]
  public void TypeConverter_IsFound(
    object primitive,
    object? value,
    bool expected )
  {
    var converter = TypeDescriptor.GetConverter( primitive.GetType() );

    converter.Should()
             .NotBeNull();
  }

#pragma warning restore xUnit1026
}
