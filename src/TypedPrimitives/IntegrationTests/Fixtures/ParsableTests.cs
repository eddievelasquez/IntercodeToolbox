// Module Name: ParsableTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class ParsableTests<TParsable, TDataFactory>
  where TParsable: IParsable<TParsable>
  where TDataFactory: ITestDataFactory
{
  #region Public Methods

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void Parse_ShouldSucceed(
    string value,
    TParsable expected,
    IFormatProvider? formatProvider )
  {
    var actual = TParsable.Parse( value, formatProvider );
    actual.Should().Be( expected );
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void TryParse_ShouldSucceed(
    string value,
    TParsable expected,
    IFormatProvider? formatProvider )
  {
    TParsable.TryParse( value, formatProvider, out var actual ).Should().BeTrue();
    actual.Should().Be( expected );
  }

  public static IEnumerable<object?[]> GetData()
  {
    return TDataFactory.GetValidValues();
  }

  #endregion
}
