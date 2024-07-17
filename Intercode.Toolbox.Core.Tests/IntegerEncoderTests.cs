// Module Name: IntegerEncoderTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Tests;

using FluentAssertions;

public class IntegerEncoderTests
{
  #region Tests

  [Theory]
  [InlineData( -42 )]
  [InlineData( -1 )]
  [InlineData( 0 )]
  [InlineData( 1 )]
  [InlineData( 42 )]
  [InlineData( IntegerEncoder.MinValue + 1 )]
  [InlineData( IntegerEncoder.MaxValue - 1 )]
  public void Encode_ShouldNotReturnSequential_WhenValuesAreSequential(
    int value )
  {
    AssertNotSequential( value );

    static void AssertNotSequential(
      int value )
    {
      var nextValue = value + 1;
      var prevValue = value - 1;
      var encodedValue = IntegerEncoder.Encode( value );
      var encodedNextValue = IntegerEncoder.Encode( nextValue );
      var encodedPrevValue = IntegerEncoder.Encode( prevValue );

      encodedValue.Should()
                  .NotBe( encodedValue + 1 );

      encodedValue.Should()
                  .NotBe( encodedValue - 1 );

      encodedValue.Should()
                  .NotBe( encodedNextValue );

      encodedValue.Should()
                  .NotBe( encodedPrevValue );
    }
  }

  [Theory]
  [InlineData( -1 )]
  [InlineData( 0 )]
  [InlineData( 1 )]
  [InlineData( IntegerEncoder.MinValue )]
  [InlineData( IntegerEncoder.MaxValue )]
  public void Encode_ShouldRoundTrip(
    int value )
  {
    var result = IntegerEncoder.Encode( value );
    var decoded = IntegerEncoder.Decode( result );

    decoded.Should()
           .Be( value );
  }

  [Theory]
  [InlineData( IntegerEncoder.MinValue - 1 )]
  [InlineData( IntegerEncoder.MaxValue + 1 )]
  public void Encode_ShouldThrow_WhenValueIsOutOfRange(
    int value )
  {
    var act = () => IntegerEncoder.Encode( value );

    act.Should()
       .Throw<ArgumentOutOfRangeException>();
  }

  #endregion
}
