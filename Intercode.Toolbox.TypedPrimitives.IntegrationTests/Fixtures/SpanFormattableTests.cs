// Module Name: SpanFormattableTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class SpanFormattableTests<TSpanFormattable, TDataFactory>
  : FormattableTests<TSpanFormattable, TDataFactory>
  where TSpanFormattable: ISpanFormattable
  where TDataFactory: ITestDataFactory
{
  #region Public Methods

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void TryFormat_WithSpan_ShouldSucceed(
    TSpanFormattable value,
    string expected,
    string? format,
    IFormatProvider? formatProvider )
  {
    Span<char> destination = stackalloc char[expected.Length * 2];
    var succeeded = value.TryFormat( destination, out var charsWritten, format, formatProvider );
    succeeded.Should().BeTrue();

    charsWritten.Should().Be( expected.Length );
    destination[..charsWritten].ToString().Should().Be( expected );
  }

  #endregion
}
