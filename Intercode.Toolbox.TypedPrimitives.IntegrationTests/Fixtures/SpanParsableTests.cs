// Module Name: SpanParsableTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class SpanParsableTests<TSpanParsable, TDataFactory>
  : ParsableTests<TSpanParsable, TDataFactory>
  where TSpanParsable: ISpanParsable<TSpanParsable>
  where TDataFactory: ITestDataFactory
{
  #region Public Methods

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void Parse_WithSpan_ShouldSucceed(
    string value,
    TSpanParsable expected,
    IFormatProvider? formatProvider )
  {
    var actual = TSpanParsable.Parse( value.AsSpan(), formatProvider );
    actual.Should().Be( expected );
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void TryParse_WithSpan_ShouldSucceed(
    string value,
    TSpanParsable expected,
    IFormatProvider? formatProvider )
  {
    TSpanParsable.TryParse( value.AsSpan(), formatProvider, out var actual ).Should().BeTrue();
    actual.Should().Be( expected );
  }

  #endregion
}
