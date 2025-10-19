// Module Name: FormattableTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class FormattableTests<TFormattable, TDataFactory>
  : ToStringTests<TFormattable, TDataFactory>
  where TFormattable: IFormattable, ITypedPrimitive
  where TDataFactory: ITestDataFactory
{
  #region Public Methods

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void ToString_WithFormatAndFormatProvider_Succeeds(
    TFormattable value,
    string? format,
    IFormatProvider? formatProvider )
  {
    // ToString of a typed primitive with value should return the ToString of the underlying value.
    value.HasValue.Should().BeTrue();
    var expected = ( ( IFormattable ) value.GetValueObject()! ).ToString( format, formatProvider );

    var actual = value.ToString( format, formatProvider );
    actual.Should().Be( expected );
  }

  public static IEnumerable<object?[]> GetData()
  {
    return TDataFactory.GetValidValues();
  }

  #endregion
}
