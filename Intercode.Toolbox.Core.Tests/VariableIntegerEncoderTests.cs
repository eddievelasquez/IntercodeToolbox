// Module Name: VariableIntegerEncoderTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Tests;

using FluentAssertions;

public class VariableIntegerEncoderTests
{
  #region Tests

  [Theory]
  [InlineData( sbyte.MinValue )]
  [InlineData( sbyte.MaxValue )]
  [InlineData( byte.MinValue )]
  [InlineData( byte.MaxValue )]
  [InlineData( short.MinValue )]
  [InlineData( short.MaxValue )]
  [InlineData( ushort.MinValue )]
  [InlineData( ushort.MaxValue )]
  [InlineData( int.MinValue )]
  [InlineData( int.MaxValue )]
  public void Encode_ShouldRoundTrip(
    int value )
  {
    var bytes = VariableIntegerEncoder.Encode( value );
    var actual = VariableIntegerEncoder.Decode( bytes );

    actual.Should()
          .Be( value );
  }

  [Fact]
  public void TryEncode_ShouldReturnFalse_WhenBufferIsTooSmall()
  {
    var buffer = new byte[1];

    VariableIntegerEncoder.TryEncode( buffer, int.MaxValue, out var bytesWritten )
                          .Should()
                          .BeFalse();

    bytesWritten.Should()
                .Be( buffer.Length );
  }

  #endregion
}
